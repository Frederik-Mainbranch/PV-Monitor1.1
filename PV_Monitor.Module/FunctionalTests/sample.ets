;#DropDB PV-MonitorEasyTest

#Application PV-MonitorWin
#Application PV-MonitorBlazor
#IfDef PV-MonitorWin

*Action About...
 
*Action OK

#EndIf

#IfDef PV-MonitorBlazor
!Action Navigation(My Details)
#EndIf
