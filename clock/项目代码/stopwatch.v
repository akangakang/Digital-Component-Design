module stopwatch(CLOCK_50, key_reset, key_start_pause, key_display_stop,
					// 时钟输入+ 3个按键；按键按下为0  板上利用施密特触发器做了一定消抖，效果待测试。
					hex0, hex1, hex2, hex3, hex4, hex5,
					// 板上的6个7段数码管，每个数码管有7位控制信号
					led0, led1, led2, led3 );
					// LED发光二极管指示灯，用于指示/测试程序按键状态，若需要，可增加。高电平亮

input CLOCK_50,key_reset,key_start_pause,key_display_stop; // 分别对应KEY012
output [6:0] hex0,hex1,hex2,hex3,hex4,hex5;
output led0,led1,led2,led3;

reg led0,led1,led2,led3;

// digits to be displayed
reg [3:0] minute_display_high;
reg [3:0] minute_display_low;
reg [3:0] second_display_high;
reg [3:0] second_display_low;
reg [3:0] msecond_display_high;
reg [3:0] msecond_display_low;

// digits to record time
reg [3:0] minute_counter_high;
reg [3:0] minute_counter_low;
reg [3:0] second_counter_high;
reg [3:0] second_counter_low;
reg [3:0] msecond_counter_high;
reg [3:0] msecond_counter_low;

reg [31:0] counter_50M; 		// 计时用计数器， 每个50MHz的clock 为20ns。
										// DE1-SOC板上有4个时钟， 都为 50MHz，所以需要500000次20ns之后，才是10ms

reg reset_time; 					// 消抖动用状态寄存器-- for reset KEY
reg [8:0] counter_reset; 		// 按键状态时间计数器
reg start_time; 					//消抖动用状态寄存器-- for counter/pause KEY
reg [8:0] counter_start; 		//按键状态时间计数器
reg display_time; 				//消抖动用状态寄存器-- for KEY_display_refresh/pause
reg [8:0] counter_display; 	//按键状态时间计数器
reg start = 1; 					// 工作状态寄存器
reg display; 						// 工作状态寄存器


// sevenseg模块为4位的BCD码至7段LED的译码器，
//下面实例化6个LED数码管的各自译码器
sevenseg LED8_minute_display_high ( minute_display_high, hex5 );
sevenseg LED8_minute_display_low ( minute_display_low, hex4 );
sevenseg LED8_second_display_high( second_display_high, hex3 );
sevenseg LED8_second_display_low ( second_display_low, hex2 );
sevenseg LED8_msecond_display_high( msecond_display_high, hex1 );
sevenseg LED8_msecond_display_low ( msecond_display_low, hex0 );

always @ (key_reset) begin
    led0 = key_reset;
end

always @ (key_start_pause) begin
    led1 = key_start_pause;
end

always @ (key_display_stop) begin
    led2 = key_display_stop;
end


// 消除按键抖动
always @(posedge CLOCK_50)
begin 
 if (!key_reset) 
 begin  // state is about to change
	counter_reset = counter_reset + 1;
   if (counter_reset == 8'b11111111) 
	begin  // signal has been in new state for long enough time
     counter_reset = 0;  // clear counter
     reset_time = 1;  // flip the state
   end
 end
 else 
	reset_time = 0;   
end


always @(posedge CLOCK_50)
begin
 if (start_time && !key_start_pause) 
 begin
   counter_start = counter_start + 1;
   if (counter_start == 8'b11111111) 
	begin
     counter_start = 0;
     start_time = ~start_time;
   end
 end else if (!start_time && key_start_pause) 
	begin
     counter_start = counter_start + 1;
     if (counter_start == 8'b11111111) 
	  begin
       counter_start = 0;
       start_time = ~start_time;

       start = !start;
     end
   end else 
	begin
     counter_start = 0;
   end
end

always @(posedge CLOCK_50)
begin
if (display_time && !key_display_stop) 
 begin
   counter_display = counter_display + 1;
   if (counter_display == 8'b11111111) 
	begin
     counter_display = 0;
     display_time = ~display_time;
   end
   end else if (!display_time && key_display_stop) 
	begin
     counter_display = counter_display + 1;
     if (counter_display == 8'b11111111) 
	  begin
       counter_display = 0;
       display_time = ~display_time;

       display = !display;
     end
   end else 
	begin
     counter_display = 0;
   end
 
end

always @ (posedge CLOCK_50) 
// 每一个时钟上升沿开始触发下面的逻辑，
// 进行计时后各部分的刷新工作
begin
	
	if(start)
	begin
	counter_50M = counter_50M + 1;
		if(counter_50M == 500000) 
		begin
		counter_50M = 0;
		msecond_counter_low = msecond_counter_low + 1; 
			if(msecond_counter_low == 10)
			begin
			msecond_counter_high = msecond_counter_high + 1;
			msecond_counter_low = 0;
		
				if(msecond_counter_high == 10)
				begin
				msecond_counter_high = 0;
				second_counter_low = second_counter_low +1;
			
					if(second_counter_low == 10)
					begin
					second_counter_low = 0;
					second_counter_high = second_counter_high +1;
				
						if(second_counter_high == 6)
						begin
						minute_counter_low = minute_counter_low +1;
						second_counter_high =0;
					
							if(minute_counter_low == 10)
							begin
							minute_counter_low = 0;
							minute_counter_high = minute_counter_high +1;
						
								if(minute_counter_high ==6)
								minute_counter_high = 0;
							end
						end
					end
				end 
			end 
		end
	end 
	
	
	if(reset_time)
	begin
	counter_50M = 0;
	msecond_counter_high = 0;
	msecond_counter_low = 0;
	second_counter_high = 0;
	second_counter_low = 0;
	minute_counter_high = 0;
	minute_counter_low = 0;
	end 
	
	if(display || reset_time)
	begin
	msecond_display_low = msecond_counter_low;
	msecond_display_high = msecond_counter_high;
	second_display_low = second_counter_low;
	second_display_high = second_counter_high;
	minute_display_low = minute_counter_low;
	minute_display_high = minute_counter_high;
	end
	
end 	 
endmodule


