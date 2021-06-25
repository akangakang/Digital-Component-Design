library verilog;
use verilog.vl_types.all;
entity sc_cu is
    port(
        op              : in     vl_logic_vector(5 downto 0);
        func            : in     vl_logic_vector(5 downto 0);
        rs              : in     vl_logic_vector(4 downto 0);
        rt              : in     vl_logic_vector(4 downto 0);
        mrn             : in     vl_logic_vector(4 downto 0);
        mm2reg          : in     vl_logic;
        mwreg           : in     vl_logic;
        ern             : in     vl_logic_vector(4 downto 0);
        em2reg          : in     vl_logic;
        ewreg           : in     vl_logic;
        z               : in     vl_logic;
        pcsource        : out    vl_logic_vector(1 downto 0);
        wpcir           : out    vl_logic;
        wreg            : out    vl_logic;
        m2reg           : out    vl_logic;
        wmem            : out    vl_logic;
        jal             : out    vl_logic;
        aluc            : out    vl_logic_vector(3 downto 0);
        aluimm          : out    vl_logic;
        shift           : out    vl_logic;
        usert           : out    vl_logic;
        sext            : out    vl_logic;
        fwdb            : out    vl_logic_vector(1 downto 0);
        fwda            : out    vl_logic_vector(1 downto 0)
    );
end sc_cu;
