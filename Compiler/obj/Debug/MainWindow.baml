   M S B A M L   `   `   `  ����  Compiler    Compiler.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          �Name$  
BaseWindow��5   	    � Title$ CMPS 455 Compiler��5   	   $	��350��6#   $	��525��60   .��5      �  � ColumnDefinitions 5   
   �� 5      $
��256* �6   �� 5      $
��261* �6   .+�5
   
   �� -   5         ��Name$ CompileButton��5
      $��Compile��6(   $
��Left=�6:   $��
10,288,0,0q�6U   $	��Top=�6i   $	��154��6�   6�   �� 5   
   -   5         ��Name$
 Code��5      $
��Left=�6    $	��273��6;   $��	10,10,0,0q�6H    �� TextWrapping$
 Wrap=�6[   $	��Top=�6o   $	��319��6�   $��2��6�    �� AcceptsReturn . $����PROGRAM  add12;
VAR
	a, b, c, d, e, f : INTEGER ;
BEGIN
	a = 13 ;
               b = 23;
               WRITELN(a) ;   
               d = 124 ;
               c = a + 2 * ( a + d ) ;
               e = c + b * d;
               f = e * c;
               WRITELN(c) ;
               WRITELN(e);
               WRITELN(f); 
END��6�   �� 5   
   -   5        $ CompilerWatch��5      $
��Left=�6)   $	��106��6D   $��	78,36,0,0q�6Q   $
 Wrap=�6d   $	��Top=�6x   $	��173��6�   $��1��6�   �� 5   
   -   5        $ CPPCode��5      $
��Left=�6#   $	��106��6>   $��
78,177,0,0q�6K   $
 Wrap=�6_   $	��Top=�6s   $	��173��6�   $��1��6�   �� 5   
   -   5        $ Clear��5      $��Clear��6    $
��Left=�60   $��175,288,0,0q�6K   $	��Top=�6`   $	��154��6x   $��2��6�   �� 5   
   -   5        $ Export��5      $��Export��6!   $
��Left=�62   $��
78,288,0,0q�6M   $	��Top=�6a   $	��173��6y   $��1��6�   �� 5   
   $��C++ Code��6   $��1��6#   $
��Left=�63   $��
78,147,0,0q�6N   $	��Top=�6b   $	��173��6z   �� 5   
   $��Compiler Watch��6   $��1��6)   $
��Left=�69   $��	78,10,0,0q�6T   $	��Top=�6g   $	��173��6   5      5      