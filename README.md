# VirtualDevice
VirtualDevice

1. 根据符号表 分析选取最合适的字节读取范围，并映射到字节数组(作为输入输出映射寄存器)，简称 映射区。
2. 各设备根据自己的符号读取/写入到 映射区 。

从映射区异步独写Plc
