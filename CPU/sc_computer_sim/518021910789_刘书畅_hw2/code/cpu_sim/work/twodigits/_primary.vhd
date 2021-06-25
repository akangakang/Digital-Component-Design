library verilog;
use verilog.vl_types.all;
entity twodigits is
    port(
        data            : in     vl_logic_vector(6 downto 0);
        leds            : out    vl_logic_vector(13 downto 0)
    );
end twodigits;
