module pipeir ( 
	pc4,ins,wpcir,clock,resetn,
	dpc4,inst );
	
	input [31:0] pc4,ins;
	input wpcir,clock,resetn;
	
	output [31:0] dpc4,inst;
	
	dffe32 pc_plus4(pc4,clock,resetn,wpcir,dpc4);
	dffe32 instruction(ins,clock,resetn,wpcir,inst);

endmodule