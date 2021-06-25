module pipedereg ( dwreg,dm2reg,dwmem,daluc,daluimm,da,db,dimm,
	drn,dshift,djal,dpc4,clock,resetn,ewreg,em2reg,ewmem,ealuc,ealuimm,
	ea,eb,eimm,ern0,eshift,ejal,epc4 );
	
	input clock,resetn;
	input dwreg,dm2reg,dwmem,daluimm,dshift,djal;
	input [31:0] da,db,dimm,dpc4;
	input [3:0] daluc;
	input [4:0] drn;
	
	output reg ewreg,em2reg,ewmem,ealuimm,eshift,ejal;
	output reg [31:0] ea,eb,eimm,epc4;
	output reg [3:0] ealuc;
	output reg [4:0] ern0;
	
	always @ (negedge resetn or posedge clock)
		begin
		if (resetn == 0) 
			begin
			ewreg  <= 0;
			em2reg <= 0;
			ewmem  <= 0;
			ealuimm<= 0;
			eshift <= 0;
			ejal   <= 0;
			ea     <= 0;
			eb     <= 0;
			eimm   <= 0;
			epc4   <= 0;
			ealuc  <= 0;
			ern0   <= 0;
			end 
		else 
			begin
			ewreg  <= dwreg;
			em2reg <= dm2reg;
			ewmem  <= dwmem;
			ealuimm<= daluimm;
			eshift <= dshift;
			ejal   <= djal;
			ea     <= da;
			eb     <= db;
			eimm   <= dimm;
			epc4   <= dpc4;
			ealuc  <= daluc;
			ern0   <= drn;
			end
		end
endmodule