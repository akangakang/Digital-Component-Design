library verilog;
use verilog.vl_types.all;
entity sc_computer is
    port(
        resetn          : in     vl_logic;
        clock           : in     vl_logic;
        pc              : out    vl_logic_vector(31 downto 0);
        inst            : out    vl_logic_vector(31 downto 0);
        aluout          : out    vl_logic_vector(31 downto 0);
        memout          : out    vl_logic_vector(31 downto 0);
        imem_clk        : out    vl_logic;
        dmem_clk        : out    vl_logic;
        io_in_sw        : in     vl_logic_vector(9 downto 0);
        io_out_led      : out    vl_logic_vector(9 downto 0);
        io_out_hex      : out    vl_logic_vector(41 downto 0)
    );
end sc_computer;
