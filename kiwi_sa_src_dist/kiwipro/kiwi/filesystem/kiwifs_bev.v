// Kiwi Scientific Acceleration of C# on FGPA
// ------------------------------------------
// (Kiwi Project:  DJ Greaves and S Singh: Compilation of parallel programs written in C# to H/W)
// (C) 2015 David J Greaves and University of Cambridge Computer Laboratory.
//
// Kiwi-substrate - RTL_SIM mode simulation implementation of substrate services.  
//                - For execution on Zynq or whatever, an alternative implementation should be substituted (not in a stable form at this time).

// This file is also provided as somethingelse.v for including in target FPGA for real use layout. ksubs3 most likely!
//

// This version was for parallel:four-phase protocol, but we are mid migrating it to HFAST/NoC

//
// This is the pre-NoC (network-on-chip) version.
//
`define Kiwi_fs_op_open     1
`define Kiwi_fs_console     2
`define Kiwi_fs_read_word   3
`define Kiwi_fs_write_word  4
`define Kiwi_fs_test_eof    5
`define Kiwi_fs_close       6
`define Kiwi_fs_length      7

`define Kiwi_stdio_fd       1
`define Kiwi_stderr_fd      2

`define Min_usr_fd          4 // Descriptors below this one are system descriptors - stdio etc..


module KIWI_BEV_FILESERVER(
    input  perform_bevfs_op_req,
    output reg perform_bevfs_op_ack, // Ack is not really needed (HSIMPLE legacy) since all ops ack in one cycle.
    output reg signed [63:0] perform_bevfs_op_return,
    input signed [63:0] perform_bevfs_op_a2,
    input signed [3:0] perform_bevfs_op_cmd,
    input clk,
    input reset);   

   parameter verbose = 0;
   parameter max_open_files = 32;		  
   integer file_handles[max_open_files-1:0];
   integer file_eofs[max_open_files-1:0];   
   integer lengths[max_open_files-1:0];
   parameter n = 100;
   
`define NULL 0

   reg [7:0] cmd_fd, free_fd;
   
   initial begin
      cmd_fd = 0;
      free_fd = `Min_usr_fd;
   end

   reg [n*8-1:0] strings;

   parameter max_file_name_length = 1024;
   reg [7:0] 	 xc, xd;
   reg 		 breakf;
   reg [max_file_name_length*8-1:0] filename;
   integer 			    max_rd_len;
   integer 			    wr_len; 			    
   integer 			    result;
   reg [7:0] 			    ptr;
   reg [63:0] 			    pbuffer;
   reg [7:0] 			    r;   
   always @(posedge clk) begin

      //$display("blog=%d", perform_bevfs_op_req);

      if (reset) begin
	 free_fd <= 0;
	 cmd_fd <= 0;
         ptr <= 0;
	 pbuffer <= 0;
	 filename <= 0;
	 end
      if (reset || !perform_bevfs_op_req) begin
	 perform_bevfs_op_ack <= 0;
	 perform_bevfs_op_return <= 0;
	 end
      
      if (perform_bevfs_op_req && !perform_bevfs_op_ack) begin // TODO delete back-to-back prohibit
	 $display("BLOG cmd=%d", perform_bevfs_op_cmd);
	 case (perform_bevfs_op_cmd)

	   `Kiwi_fs_length: // Operation 7 - get length of an open file.
	     begin
		cmd_fd = perform_bevfs_op_a2[63:56];
		perform_bevfs_op_return <= lengths[cmd_fd];
		$display("%m: cmd file length fd=%d ans=%d", cmd_fd, lengths[cmd_fd]);
		perform_bevfs_op_ack <= 1;
	     end

	   `Kiwi_fs_op_open:
	     begin
		$display("Kiwi fserver: cmd open file %d a2=%X", perform_bevfs_op_cmd, perform_bevfs_op_a2);
		case ((perform_bevfs_op_a2 >> 16) & 15)
		  1: // Subcode 1 Start
		    begin
		       free_fd <= free_fd + 1; // TODO wastes an fd on Exists check.
		       filename <= 0;
		       perform_bevfs_op_return <= 0;
		    end
		  2: // Next char of the file name
		    begin
		       filename <= (filename << 8) | (perform_bevfs_op_a2 & 8'hFF);
		       perform_bevfs_op_return <= 0;
		    end
		  3: // Do the open for read and make a note of the length and return the fd.
		    begin
		       $sformat(filename, "%0s", filename); // Discard leading nulls?
		       $display("%m: Opening for read %1s as fd=%d", filename, free_fd);
		       file_eofs[free_fd] = 0;
		       file_handles[free_fd] = $fopen(filename, "r");
		       $display("Kiwi fserver: open read verilog fd rc=%d Kiwi fd=%d", file_handles[free_fd], free_fd);

		       //
		       // Position the file pointer at end of file
		       result = $fseek(file_handles[free_fd], 0, 2);
		       if (result == -1) begin
			  $display("%m: oops, can't move file pointer");
			  $stop;
		       end
		       //
		       // Discover where it is
		       lengths[free_fd] = $ftell(file_handles[free_fd]);
		       $display("%m: File has %0d bytes", lengths[free_fd]);
		       result = $fseek(file_handles[free_fd], 0, 0); // Seek back to start
		       perform_bevfs_op_return <= (file_handles[free_fd] != 0) ? free_fd << 56 : 1;
		       end

		  4: // Open For Write
		    begin
		       $sformat(filename, "%0s", filename); // Discard leading nulls?
		       $display("%m: Opening for write %1s as fd=%d", filename, free_fd);
		       file_handles[free_fd] = $fopen(filename, "w");
		       $display("Kiwi fserver:open write verilog fd rc=%d Kiwi fd=%d", file_handles[free_fd], free_fd);
		       perform_bevfs_op_return <= (file_handles[free_fd] != 0) ? free_fd << 56 : 1;
		       end
		  5: // Check Exists
		    begin
		       $sformat(filename, "%0s", filename); // Discard leading nulls?
		       $display("Kiwi fserver: check exists %s %d", filename, free_fd);
		       xc = $fopen(filename, "r");
		       $display("Kiwi fserver:check exists ans=%d", xc);
		       if (xc != 0) $fclose(xc);
		       perform_bevfs_op_return <= (xc==0) ? 0: 1;
		       end
		    endcase
		perform_bevfs_op_ack <= 1;
	     end

	   `Kiwi_fs_read_word:
	     begin
		// Read up to seven bytes and put count in top byte.
		cmd_fd = perform_bevfs_op_a2[63:56];
		max_rd_len = perform_bevfs_op_a2[31:0];
		if (max_rd_len > 7) max_rd_len = 7;
		if (verbose >= 2) $display("Kiwi fserver: (verbose) read_word (max bytes %d) %d a2=%x cmd_fd=%d", max_rd_len, perform_bevfs_op_cmd, cmd_fd);

		breakf = 0;
		for (xc=0; !breakf && xc<max_rd_len; xc = xc+1) begin
		   if (file_eofs[cmd_fd] || $feof(file_handles[cmd_fd])) begin
		      file_eofs[cmd_fd] = 1;
		      breakf = 1;
		      xc = -1; // Part of doing a break!
		      $display("Kiwi fserver: Read eof on %d", cmd_fd);
		      end
		   else begin
		      xd = ($fgetc(file_handles[cmd_fd]) & 8'hFF);
		      pbuffer = (xc == 0) ? xd : pbuffer  | (xd << (8 * xc));
		      //$display("Kiwi fserver: pack bufferx xc=%d char=%x %s %x", xc, xd, pbuffer, pbuffer);
		      //$display("Kiwi fserver: Read xc=%d chars ' %x", xc, pbuffer);		  
		      end
		   end

		perform_bevfs_op_ack <= 1;
  	  	perform_bevfs_op_return <= { xc, pbuffer[55:0] };
	     end
	   //$display("       (file read char %c %d/%d)", perform_bevfs_op_return, ptr, pbuffer);

	   `Kiwi_fs_write_word:
	     begin
		cmd_fd = perform_bevfs_op_a2[60:56]; // five-bit field
		wr_len = perform_bevfs_op_a2[63:61];
		pbuffer = perform_bevfs_op_a2[55:0];
		if (cmd_fd < `Min_usr_fd) begin
		   //if (verbose >= 2) 
		     $display("Kiwi fserver: (verbose) write_console (max bytes %d) %d a2=%x cmd_fd=%d", wr_len, perform_bevfs_op_cmd, perform_bevfs_op_a2, cmd_fd);
		   end
		// Write up to seven bytes.
		else begin
		   if (verbose >= 2) $display("Kiwi fserver: (verbose) write_word (max bytes %d) %d a2=%x cmd_fd=%d", wr_len, perform_bevfs_op_cmd, perform_bevfs_op_a2, cmd_fd);
		   for (xc=0; xc<wr_len; xc = xc+1) begin
		      xd = pbuffer  >> (8 * xc);
		      xc = $fputc(xd, file_handles[cmd_fd]);
		      //$display("Kiwi fserver: pack bufferx xc=%d char=%x %s %x", xc, xd, pbuffer, pbuffer);
		      $display("Kiwi fserver: write xc=%d chars ' %x", xc, pbuffer);		  
		      end
		   end
		perform_bevfs_op_ack <= 1;
  	  	perform_bevfs_op_return <= { xc, 56'd0 };
	     end
	   //$display("       (file read char %c %d/%d)", perform_bevfs_op_return, ptr, pbuffer);


	   `Kiwi_fs_close:
	     begin
		cmd_fd = perform_bevfs_op_a2[63:56];
		$display("Kiwi fserver: cmd close %d a2=%x cmd_fd=%d", perform_bevfs_op_cmd, perform_bevfs_op_a2, cmd_fd);
	        $fclose(file_handles[cmd_fd]);
		perform_bevfs_op_return <= (file_handles[cmd_fd] != 0) ? 0: 2; 
		file_handles[cmd_fd] = 0;
		perform_bevfs_op_ack <= 1;
		end

	   default:
             begin
		$display("Kiwi fserver: unsupported Kiwi file system cmd %d a2=%X", perform_bevfs_op_cmd, perform_bevfs_op_a2);
		$finish(-1);
		end
	   endcase
      end
   end

                                                                                                                   

endmodule
