lw $30 $0 4
lw $29 $0 8
lw $28 $0 12
lw $27 $0 16
sw $1 $30 0
sw $2 $30 1
sw $3 $30 2
sw $4 $30 3
sw $5 $30 4
sw $6 $30 5
########### load pause button
lw $7 $30 0
# if pause not pressed
# $7 == 0 if pressed
beq $7 $0 2
xor $8 $8 $8
beq $0 $0 3
# pause pressed
bne $8 $0 1
# first press pause
xori $9 $9 1
addi $8 $8 1
########### load reset button
lw $10 $30 1
########### reset?
# $10 == 0 if pressed
bne $10 $0 6
xor $1 $1 $1
xor $2 $2 $2
xor $3 $3 $3
xor $4 $4 $4
xor $5 $5 $5
xor $6 $6 $6
########### pause?
beq $9 $0 1
j 4
########### stop watch
addi $11 $11 2
beq $11 $29 1
j 4
xor $11 $11 $11
addi $1 $1 1
# $1 == 10
bne $1 $28 2
xor $1 $1 $1
addi $2 $2 1
# $2 == 10
bne $2 $28 2
xor $2 $2 $2
addi $3 $3 1
# $3 == 10
bne $3 $28 2
xor $3 $3 $3
addi $4 $4 1
# $4 == 6
bne $4 $27 2
xor $4 $4 $4
addi $5 $5 1
# $5 == 10
bne $5 $28 2
xor $5 $5 $5
addi $6 $6 1
j 4