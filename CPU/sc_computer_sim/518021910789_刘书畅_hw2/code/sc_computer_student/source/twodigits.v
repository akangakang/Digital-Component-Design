module twodigits (data, leds);
    input  [6:0] data;
    output [13:0] leds;

    reg    [3:0]  digit0, digit1;

    sevenseg led0(digit0, leds[6:0]);
    sevenseg led1(digit1, leds[13:7]);
	 
    always @ (data) begin
        digit1 = data / 10;
        digit0 = data - data / 10 *10;
    end

endmodule // twodigit
