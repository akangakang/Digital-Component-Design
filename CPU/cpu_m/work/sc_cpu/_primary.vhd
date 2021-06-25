library verilog;
use verilog.vl_types.all;
entity sc_cpu is
    port(
        clock           : in     vl_logic;
        resetn          : in     vl_logic;
        inst            : in     vl_logic_vector(31 downto 0);
        mem             : in     vl_logic_vector(31 downto 0);
        pc              : out    vl_logic_vector(31 downto 0);
        wmem            : out    vl_logic;
        alu             : out    vl_logic_vector(31 downto 0);
        data            : out    vl_logic_vector(31 downto 0)
    );
end sc_cpu;
