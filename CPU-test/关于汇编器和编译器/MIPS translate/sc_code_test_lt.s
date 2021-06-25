addi $1 $1 128
addi $6 $6 32      # 11001010
lw $5 $1 28         # input SW0-SW9 to $5
lt $7 $5 $6
sw $7 $1 36          # output $7 to HEX0
sw $5 $1 32
j 2
