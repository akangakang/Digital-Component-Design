# $1 == 0
xor $1 $1 $1
# $30 == 0x80000000
lw $30 $0 4
############### load first number
lw $1 $30 6
############### calculate division
muli $2 $1 3
sw $1 $30 10
sw $2 $30 11
j 2