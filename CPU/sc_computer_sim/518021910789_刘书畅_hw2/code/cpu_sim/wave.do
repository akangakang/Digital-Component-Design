onerror {resume}
quietly WaveActivateNextPane {} 0
add wave -noupdate /sc_computer_sim/resetn_sim
add wave -noupdate /sc_computer_sim/clock_50M_sim
add wave -noupdate /sc_computer_sim/mem_clk_sim
add wave -noupdate /sc_computer_sim/in_port0_sim
add wave -noupdate /sc_computer_sim/in_port1_sim
add wave -noupdate /sc_computer_sim/hex0_sim
add wave -noupdate /sc_computer_sim/hex1_sim
add wave -noupdate /sc_computer_sim/hex2_sim
add wave -noupdate /sc_computer_sim/hex3_sim
add wave -noupdate /sc_computer_sim/hex4_sim
add wave -noupdate /sc_computer_sim/hex5_sim
add wave -noupdate /sc_computer_sim/led0_sim
add wave -noupdate /sc_computer_sim/led1_sim
add wave -noupdate /sc_computer_sim/led2_sim
add wave -noupdate /sc_computer_sim/led3_sim
add wave -noupdate /sc_computer_sim/pc_sim
add wave -noupdate /sc_computer_sim/inst_sim
add wave -noupdate /sc_computer_sim/aluout_sim
add wave -noupdate /sc_computer_sim/memout_sim
add wave -noupdate /sc_computer_sim/imem_clk_sim
add wave -noupdate /sc_computer_sim/dmem_clk_sim
add wave -noupdate /sc_computer_sim/out_port0_sim
add wave -noupdate /sc_computer_sim/out_port1_sim
add wave -noupdate /sc_computer_sim/mem_dataout_sim
add wave -noupdate /sc_computer_sim/data_sim
add wave -noupdate /sc_computer_sim/io_read_data_sim
add wave -noupdate /sc_computer_sim/wmem_sim
add wave -noupdate /sc_computer_sim/io_in_sw
add wave -noupdate /sc_computer_sim/io_out_hex
add wave -noupdate /sc_computer_sim/io_out_led
TreeUpdate [SetDefaultTree]
WaveRestoreCursors {{Cursor 1} {12097106 ps} 0}
quietly wave cursor active 1
configure wave -namecolwidth 225
configure wave -valuecolwidth 100
configure wave -justifyvalue left
configure wave -signalnamewidth 0
configure wave -snapdistance 10
configure wave -datasetprefix 0
configure wave -rowmargin 4
configure wave -childrowmargin 2
configure wave -gridoffset 0
configure wave -gridperiod 1
configure wave -griddelta 40
configure wave -timeline 0
configure wave -timelineunits ps
update
WaveRestoreZoom {12097084 ps} {12097154 ps}
