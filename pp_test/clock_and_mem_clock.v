module clock_and_mem_clock(
	clock_in,
	clock_out,mem_clock
);

	input clock_in;
	output reg clock_out; 
	output mem_clock;
	
	assign mem_clock = clock_in;
	
	initial
	begin
		clock_out <= 0;
	end
	
	always @ (posedge clock_in)
	begin
		clock_out <= ~clock_out;
	end
	
endmodule
	
	