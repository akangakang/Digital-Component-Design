module regfile (rna,rnb,d,wn,we,clk,clrn,qa,qb);
   input [4:0] rna,rnb,wn;
   input [31:0] d;
   input we,clk,clrn;
   
   output [31:0] qa,qb;
   
   reg [31:0] register [1:31]; // r1 - r31
   
   assign qa = (rna == 0)? 0 : register[rna]; // read
   assign qb = (rnb == 0)? 0 : register[rnb]; // read

   always @(posedge clk or negedge clrn) begin
      if (clrn == 0) begin // reset
				register[1] <= 0;
				register[2] <= 0;
				register[3] <= 0;
				register[4] <= 0;
				register[5] <= 0;
				register[6] <= 0;
				register[7] <= 0;
				register[8] <= 0;
				register[9] <= 0;
				register[10] <= 0;
				register[11] <= 0;
				register[12] <= 0;
				register[13] <= 0;
				register[14] <= 0;
				register[15] <= 0;
				register[16] <= 0;
				register[17] <= 0;
				register[18] <= 0;				
				register[19] <= 0;
				register[20] <= 0;
				register[21] <= 0;
				register[22] <= 0;
				register[23] <= 0;
				register[24] <= 0;
				register[25] <= 0;
				register[26] <= 0;
				register[27] <= 0;
				register[28] <= 0;
				register[29] <= 0;
				register[30] <= 0;
				register[31] <= 0;

      end else begin
         if ((wn != 0) && (we == 1))          // write
            register[wn] <= d;
      end
   end
endmodule