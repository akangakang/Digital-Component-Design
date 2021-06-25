/////////////////////////////////////////////////////////////
//                                                         //
// School of Software of SJTU                              //
//                                                         //
/////////////////////////////////////////////////////////////

module sc_computer (resetn,clock,pc,inst,aluout,memout,imem_clk,dmem_clk, io_in_sw, io_out_led,io_out_hex);

   input         resetn,clock;
   input  [9:0]  io_in_sw;
   output [31:0] pc,inst,aluout,memout;
   output        imem_clk,dmem_clk;
   output [41:0] io_out_hex;
	output [9:0]  io_out_led;
   wire   [31:0] data;
   wire          wmem; // all these "wire"s are used to connect or interface the cpu,dmem,imem and so on.
	wire clock_out;
	wire mem_clock_out;
	
	clock_and_mem_clock clk(clock,clock_out,mem_clock_out);
   sc_cpu cpu (clock_out,resetn,inst,memout,pc,wmem,aluout,data);          // CPU module.
   sc_instmem  imem (pc,inst,clock_out,mem_clock_out,imem_clk);                  // instruction memory.
   sc_datamem  dmem (aluout,data,memout,wmem,clock_out,mem_clock_out,dmem_clk,io_in_sw, io_out_led,io_out_hex); // data memory.

endmodule




