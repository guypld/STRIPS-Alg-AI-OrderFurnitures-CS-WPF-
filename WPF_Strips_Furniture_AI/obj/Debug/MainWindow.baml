   M S B A M L   `   `   `  ����R  NWPF_Strips_Furniture_AI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:WPF_Strips_Furniture_AIWPF_Strips_Furniture_AI  5   	   (    "WPF_Strips_Furniture_AI.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   7local%clr-namespace:WPF_Strips_Furniture_AI     5   	     � Title$  
MainWindow��5   	   $	��535��6   $
��1200��6)    � 	Resources 5      *   $WPF_Strips_Furniture_AI.MainWindowVM   	ViewModel��%   &	          5   
   6   5      .��5	      �  � RowDefinitions 5
   
   �� 5      $��* �6   �� 5      $	��100 �6   .+�5   
   � $��0��6    � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $	��400 �6#   .+�5      �� -   5        $��	MainBoard��5      $��0��6*   $��Stretch=�5      $��Stretch=�5      
�������5      � 5      $��1��6    5      �� 5      $
��Auto �6$   �� 5      $��* �6$    5      �� 5      $
��Auto �6'   �� 5       $��* �6'   .+�5#      �� $��Moves:��6   $��0��6)   $��0��66   &   System.Windows.Controls.DataGrid  5$      $��Stretch=�6   $��1��6U   $��0��6b   ��67   �� *Moves+6b   �� 5&      $��Stack(Reverse):��6   $��0��62   $��1��6?     5'      -   5        $��StackDataGrid��5'   U   $��Stretch=�6   $��1��68   $��1��6E   6U   5(      5*      � 5-   
   $��1��6    50      �� 51      $
��Auto �6#   �� 52      $
��Auto �6#   �� 53      $
��Auto �6#   �� 54      $
��Auto �6#   �� 55      $��* �6#   .+�58      �� $��0��6   $��Create New Furniture��59      .��5:      �� .+�5;      �� $
��Wrap=�6    $	��165��64    �� Padding$ 	 10 4 1 1q�6@   GEChoose Size and press 'Draw' to locate the new Furniture in the rooms5?      �� 5@       �� Orientation$ 
Horizontal=�6!   .+�5A      �� $��Size:��6    $��	 10 4 1 1q�60   �� 5B      -   5        $��txtRoomHeight��5B   "   $��23��67   $��25��6C   ��5C   "   �� *NewFurniture.Height+{�5D   "   �� *CanPutNewFurnitureState+�� 5E      -   5        $��	RotateBTN��5E   !   $��<>��62    � ToolTip$
 Swap��6E   6T   �� 5F      -   5        $��txtRoomWidth��5F   "   $��23��67   $��25��6C   ��5G   "   �� *NewFurniture.Width+{�5H   "   �� *CanPutNewFurnitureState+�� 5I      $��10��6+   �� 5J      -   5        $��btnDrawNewFurniture��5J   !   $
��Draw��6<   $��23��6K   5K   !   5L      5N      5O      �� 5Q      $��1��6   $	��A.I��5R      .��5S      �� .+�5T      �� -   5        $��lblPathFound��5T      
��� � �$��Path Found!��6C   $y�Hidden=�6Y   �� 5U      -   5        $��Start Moving��5U      $�� 10 25 10 25q�5V      {�5W      �� *CanPutNewFurnitureState+6K   5X      5Y      �� 5[      $��3��6   $��Options��5\      .��5]      �� .+�5^      �� �� 5_      -	   5        $��
chkAnimate��5_   C   $��Animate Stack Calculation��6    d� 	IsChecked$
 TrueY�6U   �� 5`      �� 5a      $ 
Horizontal=�6!   $��Center=�6:   .+�5b      �� -
   5        $
��Play��5b   !   60   �� 5c      �� 5d      -   5        $��Pause��5d   !   61   �� 5e      �� 5f      -   5        $
��Next��5f   !   60   5g      5h      5i      �� 5k      $��2��6   $��Reset��5l      .��5m      �� -   5        $��Clear Board��5m      $�� 10 25 10 25q�5n      5o      5p      5q      5s      5t      