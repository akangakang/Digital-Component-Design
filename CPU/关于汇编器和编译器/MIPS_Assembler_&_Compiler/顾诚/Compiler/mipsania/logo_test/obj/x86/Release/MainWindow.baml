   M S B A M L   `   `   `  ���� E  ALogoScript, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:LogoScriptIDELogoScriptIDE      LogoScriptIDE.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     /myspaceclr-namespace:LogoScriptIDE     -     � Title$  
Mips Mania�� � SizeToContent$ Height=� � 
ResizeMode$ CanMinimize=�$	��500�� J� CommandBindings )  #System.Windows.Input.CommandBinding  -      Command$$ myspace:MainWindow.CommandOpen��  -   $$ myspace:MainWindow.CommandSave��  -   $# myspace:MainWindow.CommandNew��  -   $$ myspace:MainWindow.CommandUndo��  -   $$ myspace:MainWindow.CommandRedo�� J� InputBindings ��  �� Command$$ myspace:MainWindow.CommandOpen��
 �� Key$ O�� �� 	Modifiers$
 Ctrlf��� $$ myspace:MainWindow.CommandSave��$ S��$
 Ctrlf��� $# myspace:MainWindow.CommandNew��$ N��$
 Ctrlf��� $$ myspace:MainWindow.CommandUndo��$ Z��$
 Ctrlf��� $$ myspace:MainWindow.CommandRedo��$ Y��$
 Ctrlf�.��]� $
��Left=�$	��Top=�.+�o� $	��Top=�$	��500��.@�m� -   $��
ui_mnuFile��$��_File��.@�m� -   $��	ui_mnuNew��$
��_New��	 m� Command$#	 myspace:MainWindow.CommandNew��
 m� InputGestureText$
 Ctrl+N���� m� -	   $��
ui_mnuOpen��$��_Open��$$	 myspace:MainWindow.CommandOpen��$
 Ctrl+O��m� -
   $��
ui_mnuSave��$
��Save��$$	 myspace:MainWindow.CommandSave��$
 Ctrl+S��m� -   $��ui_mnuSaveAs��$��_Save As���� m� -   $��ui_mnuOptions��$��_Options���� m� -   $��
ui_mnuExit��$��_Exit��m� -   $��ui_mnuEditor��$��_Editor��.@�m� -   $��
ui_mnuUndo��$��_Undo��$
 Ctrl+Z��m� -   $��
ui_mnuRedo��$��_Redo��$
 Ctrl+Y���� m� -   $��	ui_mnuCut��$
��_Cut��$
 Ctrl+X��m� -   $��
ui_mnuCopy��$��_Copy��$
 Ctrl+C��m� -   $��ui_mnuPaste��$��_Paste��$
 Ctrl+V���� m� -   $��ui_mnuCheckSyntax��$��_Check Syntax��m� -   $��
ui_mnuView��$��_View��.@�m� -   $��ui_mnuViewEditor��$��_Editor�� m� 	IsChecked . m� -   $��ui_mnuViewWatches��$��_Memory�� .  m� -   $��ui_mnuViewConsole��$��_Console�� . m� -   $��ui_mnuViewStack��$��
_Registers�� .  m� -   $��ui_mnuDebug��$��_Debug��.@�m� -   $��	ui_mnuRun��$
��_Run��m� -   $��ui_mnuPause��$��_Pause��m� -   $��
ui_mnuStop��$��_Stop���� m� -   $��ui_mnuStepin��$��Step _In��m� -   $��ui_mnuStepover��$��
Step _Over��m� -    $��
ui_mnuHelp��$��_Help��.@�m� -!   $��ui_mnuAbout��$��_About��c� $	��Top=�
�������$	��480�� � DataContext �� .@��� -"   $��
ui_toolNew��$��30��$��30��$#��myspace:MainWindow.CommandNew�� �� .���� $.��(pack://application:,,,/Component/New.png���� -#   $��ui_toolOpen��$��30��$��30��$$��myspace:MainWindow.CommandOpen�� �� .���� $/��)pack://application:,,,/Component/Open.png���� -$   $��ui_toolSave��$��30��$��30��$$��myspace:MainWindow.CommandSave�� �� .���� $/��)pack://application:,,,/Component/Save.png���� �� -%   $��ui_toolUndo��$��30��$��30��$$��myspace:MainWindow.CommandUndo�� �� .���� $/��)pack://application:,,,/Component/Undo.ico���� -&   $��ui_toolRedo��$��30��$��30��$$��myspace:MainWindow.CommandRedo�� �� .���� $/��)pack://application:,,,/Component/Redo.ico���� �� -'   $��
ui_toolRun��$��30��$��30�� �� .���� $.��(pack://application:,,,/Component/Run.png���� -(   $��ui_toolPause��$��30��$��30�� �� .���� $0��*pack://application:,,,/Component/Pause.png���� -)   $��ui_toolStop��$��30��$��30�� �� .���� $/��)pack://application:,,,/Component/Stop.png���� -*   $��ui_toolStepIn��$��30��$��30�� �� .���� $1��+pack://application:,,,/Component/StepIn.png���� -+   $��ui_toolStepOver��$��30��$��30�� �� .���� $3��-pack://application:,,,/Component/StepOver.png���� �� -,   $��ui_toolExit��$��30��$��30�� �� .���� $0��*pack://application:,,,/Component/Close.png��