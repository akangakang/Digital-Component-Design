module pipemem (we,addr,datain,clock,dmem_clk,dataout,
	io_in_sw, io_out_led,io_out_hex);

	// out_port0,out_port1,out_port2,out_port3,in_port0,in_port1,mem_dataout,io_read_data);

	input  [31:0]  addr;
   input  [31:0]  datain;

	// input [31:0] in_port0,in_port1;

	input          we, clock,dmem_clk;

	output [31:0] dataout;
	// output [31:0] out_port0,out_port1,out_port2,out_port3;
	// output [31:0] mem_dataout;
	// output [31:0] io_read_data;
	input  [9:0]  io_in_sw;
	output [41:0] io_out_hex;
	output [9:0]  io_out_led;
	
	wire           dmem_clk;    
   wire           write_enable; 
	wire [31:0] dataout;
	// wire [31:0] mem_dataout;
	wire write_data_enable;
	wire write_io_enable;
	wire   [31:0]  mem_out, io_out;
	
	assign         write_enable = we & ~clock; 
	assign write_data_enable = write_enable & (~addr[7]);
	assign write_io_enable = write_enable & addr[7];
	
	mux2x32 mem_io_dataout_mux(mem_out,io_out,addr[7],dataout);
	// mux2x32 mem_io_dataout_mux(mem_dataout,io_read_data,addr[7],dataout);
	// lpm_ram_dq_dram  dram(addr[6:2],dmem_clk,datain,write_data_enable,mem_dataout );

	// io_output io_output_reg(addr,datain,write_io_enable,dmem_clk,out_port0,out_port1,out_port2,out_port3);
	// io_input io_input_reg(addr,dmem_clk,io_read_data,in_port0,in_port1);
	lpm_ram_dq_dram  dram(addr[6:2],dmem_clk,datain, write_data_enable, mem_out );
	io_mem io(addr[6:2], dmem_clk, datain, write_io_enable, io_out, io_in_sw, io_out_led,io_out_hex);
endmodule 