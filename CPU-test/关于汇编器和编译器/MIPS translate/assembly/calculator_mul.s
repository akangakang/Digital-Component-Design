# $1 == 0
xor $1 $1 $1
# $30 == 0x80000000
lw $30 $0 4
############### load first number
lw $1 $30 4
############### load second number
lw $2 $30 5
############### calculate division
mul $3 $1 $2
sw $1 $30 6
sw $2 $30 7
sw $3 $30 8
j 2