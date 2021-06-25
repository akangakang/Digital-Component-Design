module pipeexe (ealuc,ealuimm,ea,eb,eimm,eshift,ern0,epc4,ejal,ern,ealu );

	 input ealuimm,eshift,ejal;
	 input [31:0] ea,eb,eimm,epc4;
    input [4:0] ern0;
    input [3:0] ealuc;

    output [31:0] ealu;
    output [4:0] ern;
    
    wire [31:0] alua,alub,sa,ealu0,epc8;
    wire zero;
    
	 assign sa = { 27'b0, eimm[10:6] };
	 
    mux2x32 e_alu_a(ea,sa,eshift,alua);
	 
    mux2x32 e_alu_b(eb,eimm,ealuimm,alub);
	 
	 assign epc8 = epc4 + 32'h4;
	 
    mux2x32 e_choose_epc(ealu0,epc8,ejal,ealu);
	 
    assign ern = ern0 | {5{ejal}};
    alu al_unit(alua,alub,ealuc,ealu0,zero);
endmodule