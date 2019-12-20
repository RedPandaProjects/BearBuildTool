if %CONFIGURATION%==Release if "%PLATFORM%"=="Any CPU" goto :RUN
goto :EXIT
:RUN
md temp
7z a temp\release.7z bin\release
:EXIT