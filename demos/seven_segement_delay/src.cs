//
// Kiwi Scientific Acceleration - Simple 7-segment display driver - net level
// (C) 2008 D J Greaves, University of Cambridge Computer Laboratory.

// 


using System;
using System.Text;
using KiwiSystem;


public static class seven_segment
{
  const int n_digits = 4;
  
   
  [Kiwi.InputBitPort("clear")] static bool clear;
  [Kiwi.OutputWordPort("segments")] static uint segments;
  [Kiwi.OutputWordPort("strobes")] static uint strobes;

  // Digit data in an array
  static int [] digit_data = new int [n_digits];

  // Seven segment character set ROM
  // Digits 0-9 and a minus sign. Bit g is lsb, zero for on.
  static int [] cbg_seven_seg = new int [] { 0x01, 0x4f, 0x12, 0x06, 0x4c, 0x24, 0x60, 0x0f, 0x00, 0x0c, 0x7E };


  static int seven_seg_encode_data(int d)
  {
    return cbg_seven_seg[d];
  }


  static int digit_on_display = 0;


  static void scan()
  {
    digit_on_display = (digit_on_display + 1) % n_digits;
    strobes = 1u << digit_on_display;
    segments = (uint)(seven_seg_encode_data(digit_data[digit_on_display]));
    for (int delay=0;delay<4; delay++) Kiwi.Pause();
  }


  static public void reset_data()
  {
    for (int i=0; i < n_digits; i++)
      {
	digit_data[i] = 0;	  
      }
  }

  static void increment_bcd()
  {
    for (int d=0; d
