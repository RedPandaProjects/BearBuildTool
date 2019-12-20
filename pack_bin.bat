if %CONFIGURATION%==Release if %PLATFORM%==Any goto :RUN
goto :EXIT
:RUN
md temp
cd bin\Debug
copy *.* ..\..\temp
cd ..\..\temp
copy ..\Licence.txt 
7z a Release.7z .\*
:EXIT