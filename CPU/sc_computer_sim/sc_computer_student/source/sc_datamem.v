module sc_datamem (addr, datain, dataout, we, clock, mem_clk, dmem_clk, io_in_sw, io_out_led,io_out_hex);

   input  [31:0]  addr;
   input  [31:0]  datain;
   input          we, clock, mem_clk;
   input  [9:0]   io_in_sw;
   output [31:0]  dataout;
   output         dmem_clk;
   output [9:0]  io_out_led;
	output [41:0]  io_out_hex;

   wire           dmem_clk, dram_write_enable, io_write_enable;
   wire   [31:0]  mem_out, io_out;

   assign         dram_write_enable = we & ~clock & ~addr[7];
   assign         io_write_enable = we & ~clock & addr[7];
   assign         dmem_clk = mem_clk & ( ~ clock);
   assign         dataout = addr[7] ? io_out : mem_out;

   lpm_ram_dq_dram  dataram(addr[6:2], dmem_clk, datain, dram_write_enable, mem_out);
   io_mem io(addr[6:2], dmem_clk, datain, io_write_enable, io_out, io_in_sw, io_out_led,io_out_hex);
endmodule
