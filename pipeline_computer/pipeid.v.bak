module pipeid (mwreg,mrn,ern,ewreg,em2reg,mm2reg,dpc4,inst,
	wrn,wdi,ealu,malu,mmo,wwreg,clock,resetn,
	bpc,jpc,pcsource,wpcir,dwreg,dm2reg,dwmem,daluc,
	daluimm,da,db,dimm,drn,dshift,djal);
	
	input clock,resetn;
	input [31:0] dpc4,inst,wdi,ealu,malu,mmo;
	input [4:0] ern,mrn,wrn;
	input mwreg,ewreg,em2reg,mm2reg,wwreg;
	
	output [31:0] bpc,jpc,da,db,dimm;
	output [4:0] drn;
	output [3:0] daluc;
	output [1:0] pcsource;
	output wpcir,dwreg,dm2reg,dwmem,daluimm,dshift,djal;
	
	wire [5:0] op,func;
	wire [4:0] rs,rt,rd;
	wire [31:0] qa,qb;
	wire [1:0] fwda,fwdb;
	wire  usert,sext,rsrtequ,e;
	
	//cut inst
	assign op = inst[31:26];
	assign func = inst[5:0];
	assign rs = inst[25:21];
	assign rt = inst[20:16];
	assign rd = inst[15:11];
	
	//cu
	sc_cu cu(op,func,rs,rt,mrn,mm2reg,mwreg,ern,em2reg,ewreg,
	rsrtequ,pcsource,wpcir,dwreg,dm2reg,dwmem,djal,daluc,daluimm,
	dshift,usert,sext,fwdb,fwda);
	
	//regfile
	regfile regf(rs,rt,wdi,wrn,wwreg,~clock,resetn,qa,qb);
	
	//output:drn
	mux2x5 rd_rt(rd,rt,usert,drn);
	
	//output:da
	mux4x32 alu_a (qa,ealu,malu,mmo,fwda,da);
	
	//output:db
	mux4x32 alu_b (qb,ealu,malu,mmo,fwdb,db);
	
	//output:jpc = dpc4[31..28] + (addr << 2)[27..0]
	assign jpc = {dpc4[31:28],inst[25:0],2'b00};
	
	//output:pcsource
	assign pcsource = pcsource;
	
	//wire:rsrtequ = (da == db)
	assign rsrtequ = ~|(da^db);
	
	//wire:e
	assign e = sext & inst[15];
	
	//output:dimm = sigend extent inst[15..0]
	assign dimm = {{16{e}},inst[15:0]};
	
	//output:bpc = dpc4 + dimm << 2
	assign bpc = dpc4 + {dimm[29:0],2'b00};
	
endmodule