# $30 == 0x80000000
lw $30 $0 4
############### load first number
lw $1 $30 4
############### load second number
lw $2 $30 5
############### calculate product
xor $3 $3 $3
xor $4 $4 $4
# determine $3
beq $1 $2 2
bl $1 $2 1
addi $3 $3 1
# determine $4
beq $1 $2 2
bg $1 $2 1
addi $4 $4 1
sw $1 $30 6
sw $2 $30 7
sw $3 $30 4
sw $4 $30 5
j 1