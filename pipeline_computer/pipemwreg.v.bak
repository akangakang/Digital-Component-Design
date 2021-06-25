module pipemwreg(mwreg,mm2reg,mmo,malu,mrn,clk,clrn,wwreg,wm2reg,wmo,walu,wrn);

    input clk,clrn;
	 input [4:0] mrn;
    input [31:0] mmo,malu;
    input mwreg,mm2reg;
    
    output reg [31:0] wmo,walu;
    output reg [4:0] wrn;
    output reg wwreg,wm2reg;
    
    always @ (negedge clrn or posedge clk)
		begin
		if(clrn == 0) 
			begin
			wwreg   <= 0;
			wm2reg  <= 0;
			wmo     <= 0;
			walu    <= 0;
			wrn     <= 0;
			end 
		else 
			begin 
			wwreg   <= mwreg ;
			wm2reg  <= mm2reg;
			wmo     <= mmo   ;
			walu    <= malu  ;
			wrn     <= mrn   ;
			end
		end
		
endmodule 