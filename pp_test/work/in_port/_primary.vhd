library verilog;
use verilog.vl_types.all;
entity in_port is
    port(
        sw9             : in     vl_logic;
        sw8             : in     vl_logic;
        sw7             : in     vl_logic;
        sw6             : in     vl_logic;
        sw5             : in     vl_logic;
        sw4             : in     vl_logic;
        sw3             : in     vl_logic;
        sw2             : in     vl_logic;
        sw1             : in     vl_logic;
        sw0             : in     vl_logic;
        \out\           : out    vl_logic_vector(31 downto 0)
    );
end in_port;
