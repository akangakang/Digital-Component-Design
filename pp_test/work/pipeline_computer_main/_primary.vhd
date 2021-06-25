library verilog;
use verilog.vl_types.all;
entity pipeline_computer_main is
    port(
        resetn          : in     vl_logic;
        clock           : in     vl_logic;
        mem_clock       : in     vl_logic;
        pc              : out    vl_logic_vector(31 downto 0);
        inst            : out    vl_logic_vector(31 downto 0);
        ealu            : out    vl_logic_vector(31 downto 0);
        malu            : out    vl_logic_vector(31 downto 0);
        walu            : out    vl_logic_vector(31 downto 0);
        io_in_sw        : in     vl_logic_vector(9 downto 0);
        io_out_led      : out    vl_logic_vector(9 downto 0);
        io_out_hex      : out    vl_logic_vector(41 downto 0)
    );
end pipeline_computer_main;
