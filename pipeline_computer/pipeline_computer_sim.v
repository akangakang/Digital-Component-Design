`timescale 1ps/1ps
module pipelined_computer_sim;
	reg resetn_sim, clock_sim, mem_clock_sim;
	// reg [31:0] in_port0,in_port1;
	reg [9:0] io_in_sw_sim;
	wire [31:0] pc_sim, inst_sim, ealu_sim, malu_sim, walu_sim;
	// wire [31:0] out_port0,out_port1,out_port2,out_port3,mem_dataout,io_read_data;
	wire [41:0] io_out_hex_sim;
	wire [9:0]  io_out_led_sim;
	pipeline_computer pipeline_computer_main_instance(resetn_sim, clock_sim, mem_clock_sim, pc_sim, inst_sim, ealu_sim, malu_sim, walu_sim,		
																				io_in_sw_sim, io_out_led_sim,io_out_hex_sim);
																				//out_port0,out_port1,out_port2,out_port3,in_port0,in_port1,mem_dataout,io_read_data);
	initial begin
		clock_sim = 1;
		while (1)
			#2 clock_sim = ~clock_sim;
	end
	initial begin
		mem_clock_sim = 0;
		while (1)
			#2 mem_clock_sim = ~mem_clock_sim;
	end
	initial begin
		resetn_sim = 0;
		while (1)
			#5 resetn_sim = 1;
	end
	initial begin
		io_in_sw_sim = 10'b0000100001;
		// while (1)
		// 	#60 io_in_sw_sim = io_in_sw_sim + 1;
	end
	
    initial begin
		$display($time, "resetn=%b clock=%b mem_clock=%b", resetn_sim, clock_sim, mem_clock_sim);
	end
endmodule
