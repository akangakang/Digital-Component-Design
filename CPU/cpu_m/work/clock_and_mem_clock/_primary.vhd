library verilog;
use verilog.vl_types.all;
entity clock_and_mem_clock is
    port(
        clock_in        : in     vl_logic;
        clock_out       : out    vl_logic;
        mem_clock       : out    vl_logic
    );
end clock_and_mem_clock;
