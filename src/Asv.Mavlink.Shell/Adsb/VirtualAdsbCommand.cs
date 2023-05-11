﻿using System;
using System.Reactive.Concurrency;
using Asv.Common;
using Asv.Mavlink.V2.Common;
using Asv.Mavlink.Vehicle;
using ManyConsole;

namespace Asv.Mavlink.Shell;

public class VirtualAdsbCommand : ConsoleCommand
{
    private string _cs = "tcp://127.0.0.1:7344?server=true";
    private string _start = "-O54.2905802,63.6063891,500";
    private string _end = "-O64.2905802,83.6063891,500";
    private string _callSign = "UFO";
    private bool _isStoped;

    public VirtualAdsbCommand()
    {
        IsCommand("adsb", "Generate virtual ADSB Vehicle");
        HasOption("cs=", $"Mavlink connection string. By default '{_cs}'", _ => _cs = _);
        HasOption("start=", $"GeoPoint from where vehicle starts flying. Default value is {_start}", _ => _start = _);
        HasOption("end=", $"GeoPoint where vehicle stops flight. Default value is {_end}", _ => _end = _);
        HasOption("callSign=", $"Vehicle call sign. Default value is {_callSign}", _ => _callSign = _);
        HasOption("stop", $"Stop vehicle. Default value is {_isStoped}", _ => _isStoped = true);
    }

    private static GeoPoint SetGeoPoint(string value)
    {
        var values = value.Split(',');

        if (values.Length is >= 3 and <= 3)
            return new GeoPoint(Convert.ToDouble(value[0]), Convert.ToDouble(value[1]), Convert.ToDouble(value[2]));
        
        Console.WriteLine($"Incorrect GeoPoint string fromat");
        return new GeoPoint();
    }
    
    public override int Run(string[] remainingArguments)
    {
        var start = SetGeoPoint(_start);
        var end = SetGeoPoint(_end);

        var distance = GeoMath.Distance(start, end);
        var azimuth = GeoMath.Azimuth(start, end);

        var server = new AdsbVehicleServer(
            new MavlinkV2Connection(_cs, _ =>
            {
                _.RegisterCommonDialect();
            }),
            new MavlinkServerIdentity{ComponentId = 13, SystemId = 13}, 
            new PacketSequenceCalculator(),
            Scheduler.Default, 
            new AdsbVehicleServerConfig());

        while (!_isStoped)
        {
            for (var i = 0; i < distance; i++)
            {
                var nextPoint = start.RadialPoint(10 * i, azimuth);
            
                server.Set(_ =>
                {
                    _.Altitude = (int)nextPoint.Altitude;
                    _.Lon = (int)nextPoint.Longitude;
                    _.Lat = (int)nextPoint.Latitude;
                    _.Callsign = _callSign.ToCharArray();
                    _.Flags = AdsbFlags.AdsbFlagsSimulated;
                    _.Squawk = 15;
                    _.Heading = 13;
                    _.AltitudeType = AdsbAltitudeType.AdsbAltitudeTypeGeometric;
                    _.EmitterType = AdsbEmitterType.AdsbEmitterTypeNoInfo;
                    _.HorVelocity = 150;
                    _.VerVelocity = 75;
                    _.IcaoAddress = 1313;
                });
            }
        }

        return -1;
    }
}