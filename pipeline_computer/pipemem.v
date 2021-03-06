module pipemem (we,addr,datain,clock,dmem_clk,dataout,
	io_in_sw, io_out_led,io_out_hex);

	
	input  [31:0]  addr;
   input  [31:0]  datain;
	
	input          we, clock,dmem_clk;

	output [31:0] dataout;
	input  [9:0]  io_in_sw;
	output [41:0] io_out_hex;
	output [9:0]  io_out_led;
	
	
	
	wire           dmem_clk, dram_write_enable, io_write_enable;
	wire   [31:0]  mem_out, io_out;
	
	assign         dram_write_enable = we & ~clock & ~addr[7];
   assign         io_write_enable = we & ~clock & addr[7];
//    assign         dataout = addr[7] ? io_out : mem_out;
	mux2x32 mem_io_dataout_mux(mem_out,io_out,addr[7],dataout);

	lpm_ram_dq_dram  dram(addr[6:2],dmem_clk,datain, dram_write_enable, mem_out );
	io_mem io(addr[6:2], dmem_clk, datain, io_write_enable, io_out, io_in_sw, io_out_led,io_out_hex);
	

endmodule 