addi $1 $1 128
addi $6 $6 195      # 11001010
lw $5 $1 28         # input SW0-SW7 to $5
hamdis $7 $6 $5
sw $7 $1 36          # output $7 to HEX5
sw $5 $1 32
j 2
