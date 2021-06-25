addi $1 $1 128
lw $5 $1 20         # input inport0 to $4
lw $6 $1 24         # input inport1 to $5
add $7 $5 $6        # add inport0 with inport1 to $6
sw $5 $1 4         # output inport0 to outport0
sw $5 $1 12
sw $6 $1 8         # output inport1 to outport1
sw $6 $1 16
sw $7 $1 0         # output result to outport2
j 1
