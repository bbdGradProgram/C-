using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.RDS;
using Amazon.CDK.AWS.ElasticBeanstalk;
using Constructs;

namespace Infrastructure
{
    public class InfrastructureStack : Stack
    {
        internal InfrastructureStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // The code that defines your stack goes here
            //////////////////// DATABASE
            const int dbPort = 3214;

            var vpc = new Vpc(this, "WMVPC", new VpcProps {
                IpAddresses = IpAddresses.Cidr("10.0.0.0/16"),
                SubnetConfiguration = new ISubnetConfiguration[] {
                    new SubnetConfiguration {
                        Name = "WMPublicSubnet",
                        SubnetType = SubnetType.PUBLIC,
                        CidrMask = 24
                    }
                },
                MaxAzs = 4
            });

            var db = new DatabaseInstance(this, "WMDB", new DatabaseInstanceProps {
                InstanceIdentifier = "WMPostgresqlDBInstance",
                InstanceType = Amazon.CDK.AWS.EC2.InstanceType.Of(InstanceClass.BURSTABLE3, InstanceSize.MICRO),
                Vpc = vpc,
                VpcSubnets = new SubnetSelection {
                    SubnetType = SubnetType.PUBLIC
                },
                Port = dbPort,
                Engine = DatabaseInstanceEngine.POSTGRES
            });

            db.AddRotationSingleUser();
            db.Connections.AllowDefaultPortFromAnyIpv4();



            //////////////////// API & WebClient 
            // security groups
            var mySecurityGroupWithoutInlineRules = new SecurityGroup(this, "WMSecurityGroup", new SecurityGroupProps {
                Vpc = vpc,
                Description = "Allow access to ec2 instances",
                AllowAllOutbound = true,
                DisableInlineRules = true
            });
            // This will add the rule as an external cloud formation construct
            mySecurityGroupWithoutInlineRules.AddIngressRule(Peer.AnyIpv4(), Port.AllTraffic(), "allow access from the world");  
        
            // keypair
            var keyPair = KeyPair.FromKeyPairAttributes(this, "WMKeyPair", new KeyPairAttributes {
                KeyPairName = "WMSSHKeyPair",
                Type = KeyPairType.RSA
            });

            // EC2
            var WMApiEC2Instance = new Instance_(this, "WMApiInstance", new Amazon.CDK.AWS.EC2.InstanceProps {
                InstanceType = new Amazon.CDK.AWS.EC2.InstanceType("t2.micro"),
                MachineImage = MachineImage.LatestAmazonLinux2023(),
                Vpc = vpc,
                KeyPair = keyPair,
                SecurityGroup = mySecurityGroupWithoutInlineRules
            });

            var WMWebEC2Instance = new Instance_(this, "WMWebInstance", new Amazon.CDK.AWS.EC2.InstanceProps {
                InstanceType = new Amazon.CDK.AWS.EC2.InstanceType("t2.micro"),
                MachineImage = MachineImage.LatestAmazonLinux2023(),
                Vpc = vpc,
                KeyPair = keyPair,
                SecurityGroup = mySecurityGroupWithoutInlineRules
            });
        }
    }
}
