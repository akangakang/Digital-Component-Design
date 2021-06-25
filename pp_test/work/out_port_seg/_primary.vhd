library verilog;
use verilog.vl_types.all;
entity out_port_seg is
    port(
        \in\            : in     vl_logic_vector(31 downto 0);
        out1            : out    vl_logic_vector(6 downto 0);
        out0            : out    vl_logic_vector(6 downto 0)
    );
end out_port_seg;
