module twodigits (data, leds);
    input  [31:0] data;
    output [13:0] leds;

    reg    [3:0]  digit0, digit1;

    sevenseg led0(digit0, leds[6:0]);
    sevenseg led1(digit1, leds[13:7]);

	 wire [6:0] trunc_data;
	 assign trunc_data = data - data / 100 * 100;
	 
    always @ (data) begin
        digit1 = trunc_data / 10;
        digit0 = trunc_data - trunc_data / 10;
    end

endmodule // twodigit
