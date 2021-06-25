module pipepc(
	npc,wpcir,clock,resetn,
	pc);
	
   input  [31:0] npc;
   input  wpcir,clock,resetn;
	
   output [31:0] pc;
   dffe32 next_pc(npc, clock, resetn, wpcir, pc);

endmodule