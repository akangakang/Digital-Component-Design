# func for Inst_R
INST_R = {"add":"100000", 
          "sub":"100010", 
          "and":"100100",
          "or" :"100101",
          "xor":"100110",
          "sll":"000000",
          "srl":"000010",
          "sra":"000011",
          "jr" :"001000"}

# op for Inst_I and Inst_J
INST_I = {"addi":"001000",
          "andi":"001100",
          "ori" :"001101",
          "xori":"001110",
          "lw":"100011",
          "sw"  :"101011",
          "beq" :"000100",
          "bne" :"000101",
          "lui" :"001111"}

INST_J = {"j"  :"000010",
          "jal":"000011"}

Final = []

def decodeR(line_num, reg):
    if (reg[0] != '$'):
        print "Error at line " + line_num
    ret = bin(int(reg[1:]))[2:]
    return '0' * (5 - len(ret)) + ret

def decodeSA(line_num, sa):
    ret = bin(int(sa))[2:]
    return '0' * (5 - len(ret)) + ret

def decodeIMM(line_num, imm):
    ret = bin((int(imm) + 0x10000) & 0xffff)[2:]
    return '0' * (16 - len(ret)) + ret

def decodeADDR(line_num, addr):
    ret = bin(int(addr))[2:]
    return '0' * (26 - len(ret)) + ret

def parseRinst(line_num, parts):
    op = '000000'
    rs = '00000'
    rt = '00000'
    rd = '00000'
    sa = '00000'

    if (parts[0] == 'jr'):
        rs = decodeR(line_num, parts[1])
    if (parts[0] == 'sll' or parts[0] == 'srl' or parts[0] == 'sra'):
        rd = decodeR(line_num, parts[1])
        rt = decodeR(line_num, parts[2])
        sa = decodeSA(line_num, parts[3])  #bin(parts[3])[2:]
    if (parts[0] == 'xor' or parts[0] == 'or' or parts[0] == 'and' or parts[0] == 'sub' or parts[0] == 'add'):
        rd = decodeR(line_num, parts[1])
        rs = decodeR(line_num, parts[2])
        rt = decodeR(line_num, parts[3])

    return op + rs + rt + rd + sa + INST_R[parts[0]]

def parseIinst(line_num, parts):
    op = INST_I[parts[0]]

    if (parts[0] == 'lui'):
        rs = '00000'
        rt = decodeR(line_num, parts[1])
        imm = decodeIMM(line_num, parts[2])
        return op + rs + rt + imm
    else:
        rt = decodeR(line_num, parts[1])
        rs = decodeR(line_num, parts[2])
        imm = decodeIMM(line_num, parts[3])
        return op + rs + rt + imm

def parseJinst(line_num, parts):
    op = INST_J[parts[0]]
    addr = decodeADDR(line_num, parts[1])
    return op + addr

def parseLine(line_num, line):
    parts = line.split(' ')
    print parts[0]
    if INST_R.has_key(parts[0]):
        Final.append(parseRinst(line_num, parts))
    else:
        if INST_I.has_key(parts[0]):
            Final.append(parseIinst(line_num, parts))
        else:
            if INST_J.has_key(parts[0]):
                Final.append(parseJinst(line_num, parts))
            else:
                print "Error at line ", line_num

def main():
    src_name = raw_input("src file: ")
    src_file = open(src_name)
    line_num = 0

    while True:
        line = src_file.readline()
        line_num += 1
        if (line == ''):
            break
        # not comment
        if (line[0] != '#'):
            parseLine(line_num, line)

    out = open(src_name.split('.')[0]+".mif", "w")
    out.write("DEPTH = 4096;\n")
    out.write("WIDTH = 32;\n")
    out.write("ADDRESS_RADIX = HEX;\n")
    out.write("DATA_RADIX = HEX;\n")

    out.write("CONTENT\n")
    out.write("BEGIN\n")

    line_num = 0
    import string
    for entry in Final:
        num = string.atoi(entry, 2)
        out.write("%2x : %08x;\n" % (line_num, num))
        line_num += 1
       # print entry

    out.write("END;\n")

if __name__ == '__main__':
    main()