library verilog;
use verilog.vl_types.all;
entity dffe32 is
    port(
        d               : in     vl_logic_vector(31 downto 0);
        clk             : in     vl_logic;
        clrn            : in     vl_logic;
        e               : in     vl_logic;
        q               : out    vl_logic_vector(31 downto 0)
    );
end dffe32;
