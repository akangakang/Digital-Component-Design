library verilog;
use verilog.vl_types.all;
entity io_mem is
    port(
        addr            : in     vl_logic_vector(3 downto 0);
        clock           : in     vl_logic;
        data_in         : in     vl_logic_vector(31 downto 0);
        write_enable    : in     vl_logic;
        data_out        : out    vl_logic_vector(31 downto 0);
        io_in_sw        : in     vl_logic_vector(9 downto 0);
        io_out_led      : out    vl_logic_vector(9 downto 0);
        io_out_hex      : out    vl_logic_vector(41 downto 0)
    );
end io_mem;
