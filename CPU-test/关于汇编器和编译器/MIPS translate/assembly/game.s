# load all numbers needed
lw $1 $0 0
lw $2 $0 4
lw $3 $0 8
lw $4 $0 12
lw $5 $0 16
lw $6 $0 20
lw $7 $0 24
lw $8 $0 28
lw $9 $0 32
lw $10 $0 36
lw $11 $0 40
lw $12 $0 44
lw $13 $0 48
lw $14 $0 52
lw $15 $0 56
lw $16 $0 60
lw $30 $0 64
# move the ball
add $1 $1 $9
add $2 $2 $10
add $3 $3 $9
add $4 $4 $10
# ensure ball is in view
# Vertical, $1
#   if $1 == $11 or $1 == $12 then
#      $9 = -$9
bne $1 $11 1
addi $9 $0 1
bne $1 $12 1
addi $9 $0 -1
# Horizonal, $2
#   if $2 == $13 or $2 == $14 then
#      $10 = -$10
bne $2 $13 1
addi $10 $0 1
bne $2 $14 1
addi $10 $0 -1
# prepare for board moving
addi $18 $0 1
addi $19 $0 800
# $18 == 1, $19 == 800
# Read button0  right
lw $17 $30 1
beq $17 $0 3
beq $8 $19 2
addi $6 $6 1
addi $8 $8 1
# Read button1  left
lw $17 $30 0
beq $17 $0 3
beq $6 $18 2
addi $6 $6 -1
addi $8 $8 -1
#
# judge if Game Over
bne $1 $12 9
addi $20 $20 1
sub $21 $2 $8
srl $21 $21 31
bne $21 $0 1
# !!! this address should be fixed by hand
j 0
sub $21 $6 $4
srl $21 $21 31
bne $21 $0 1
j 0
#
# output location
sw $1 $30 10
sw $2 $30 11
sw $3 $30 12
sw $4 $30 13
sw $5 $30 14
sw $6 $30 15
sw $7 $30 16
sw $8 $30 17
# output score
sw $20 $30 9
# loop
xor $29 $29 $29
# $28 is idle time
lw $28 $0 68
addi $29 $29 1
bne $29 $28 -2
j 17