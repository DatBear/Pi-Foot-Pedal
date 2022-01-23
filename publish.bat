dotnet publish -r linux-arm --no-self-contained
plink -v -batch -pw "testing" -ssh pi@raspberrypi mkdir /home/pi/PiFootPedal
pscp -pw "testing" -r ./PiFootPedal/bin/Debug/net6.0/linux-arm/publish/* pi@raspberrypi:/home/pi/PiFootPedal
plink -v -batch -pw "testing" -ssh pi@raspberrypi chmod u+x,o+x /home/pi/PiFootPedal/PiFootPedal