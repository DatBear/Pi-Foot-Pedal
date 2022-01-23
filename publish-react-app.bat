cd PiFootPedal/ClientApp
npm run build
plink -v -batch -pw "testing" -ssh pi@raspberrypi mkdir /home/pi/PiFootPedal/ClientApp/build
pscp -pw "testing" -r ./build/* pi@raspberrypi:/home/pi/PiFootPedal/ClientApp/build
pause