   M S B A M L   `   `   `  ����  
LogoScript"    LogoScriptIDE.WatchesWindow1   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Watch��5   	   $	��250��6   $	��450��6$    � WindowStyle$ 
ToolWindow=�6I    � ShowInTaskbar .  .��5      � .+�5   
   &   System.Windows.Controls.DataGrid  -   5        $��ui_watchesgrid��5
      
������    AutoGenerateColumns .  $��Stretch=�5	   )   $��Stretch=�5         CanUserReorderColumns .     
IsReadOnly .  $��	0,1,188,0q�5          Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      -   5        ui_nameColumn��,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 5   =      CanUserResize . 	   Header$
	 Name��6_   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 6m   �� *Name+6�     5      -   5        ui_valueColumn��$
 Auto 5   =    . $	 Value��6_   
 6n   �� *Value+6�     5      -   5        ui_remarksColumn��$
 Auto 5   ?    . $	 Remarks��6a   
 6r   �� *	Remarks+6�   5      �� 5   
   -   5        $��
ui_addLine��5   &   $��	Add Watch��6   $��	261,2,0,3q�6Q   $
��Left=�6d   $��78��6   �� 5   
   -   5        $��ui_deleteLine��5   (   $��Delete Watch��6   $��0,1,0,3q�6=   $��Right=�6j   $��83��6�   5      5      