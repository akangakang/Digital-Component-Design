library verilog;
use verilog.vl_types.all;
entity sc_computer is
    port(
        resetn          : in     vl_logic;
        clock           : in     vl_logic;
        mem_clk         : in     vl_logic;
        pc              : out    vl_logic_vector(31 downto 0);
        inst            : out    vl_logic_vector(31 downto 0);
        aluout          : out    vl_logic_vector(31 downto 0);
        memout          : out    vl_logic_vector(31 downto 0);
        imem_clk        : out    vl_logic;
        dmem_clk        : out    vl_logic;
        out_port0       : out    vl_logic_vector(31 downto 0);
        out_port1       : out    vl_logic_vector(31 downto 0);
        out_port2       : out    vl_logic_vector(31 downto 0);
        in_port0        : in     vl_logic_vector(31 downto 0);
        in_port1        : in     vl_logic_vector(31 downto 0);
        mem_dataout     : out    vl_logic_vector(31 downto 0);
        io_read_data    : out    vl_logic_vector(31 downto 0)
    );
end sc_computer;
