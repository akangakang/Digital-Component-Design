module io_mem (addr, clock, data_in, write_enable, data_out,io_in_sw, io_out_led,io_out_hex);
    input [3:0]  addr;
	 input [31:0] data_in;
	 input        clock, write_enable;
	 input [9:0]  io_in_sw;
    output reg [31:0] data_out;
	 output reg [41:0] io_out_hex;
	 output reg [9:0] io_out_led;
	 
    wire [13:0] hexs;
	
	 
    twodigits hex_two(data_in[5:0], hexs);		// 把data_in 对应的写道hex里
	
	 
    always @ (posedge clock) begin
		  
		  if (write_enable) begin
            case (addr)
                0: io_out_hex[13:0] = hexs;     // led hex0, hex1
                1: io_out_hex[27:14] = hexs;   // led hex2, hex3
                2: io_out_hex[41:28] = hexs;   // led hex4, hex3
					 3: io_out_led[4:0] = data_in[4:0];
					 4: io_out_led[9:5] = data_in[4:0];
                default:
						begin
						io_out_led = 10'b1111111111;  // all leds off
						io_out_hex = 0	;	// all hexs off
						end
            endcase
        end

        case (addr)
            5: data_out = {27'b0, io_in_sw[4:0]};    // switch 0-4
            6: data_out = {27'b0, io_in_sw[9:5]};    // switch 5-9
       
				default: data_out = 0;
        endcase
    end
endmodule //simple_ i_addr, data_in, write_enable, sdata_out
