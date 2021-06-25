library verilog;
use verilog.vl_types.all;
entity pipemem is
    port(
        we              : in     vl_logic;
        addr            : in     vl_logic_vector(31 downto 0);
        datain          : in     vl_logic_vector(31 downto 0);
        clock           : in     vl_logic;
        dmem_clk        : in     vl_logic;
        dataout         : out    vl_logic_vector(31 downto 0);
        io_in_sw        : in     vl_logic_vector(9 downto 0);
        io_out_led      : out    vl_logic_vector(9 downto 0);
        io_out_hex      : out    vl_logic_vector(41 downto 0)
    );
end pipemem;
