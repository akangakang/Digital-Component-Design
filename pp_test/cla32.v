module cla32 (a,b,sub,s);
	input [31:0] a,b;
	input sub;
	output [31:0] s;
	assign s = sub ? a - b : a + b;
endmodule 