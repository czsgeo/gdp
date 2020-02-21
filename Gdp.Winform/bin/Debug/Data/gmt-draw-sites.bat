REM show all sites

gmt begin layers png,pdf

REM 绘制地形起伏底图
gmt basemap -JH180/15c -Rg -B0
gmt grdimage @earth_relief_10m -Cetopo1 -I+d
gmt colorbar -Bxa2000f+l"Elevation (m)"

REM 绘制震中位置
REM echo 130.72 32.78 >  tmp.dat  
REM gmt plot -Sa0.5c -W0.5p,black,solid -Gyellow tmp.dat


REM 绘制台站位置
gmt plot -Sc0.1c -W0.5p,black,solid -Gblack SiteInfo.txt.xls   
REM gmt text SiteNames.txt.xls
gmt end show

