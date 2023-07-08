using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    internal class QHierarchyResources
    {
        private static QHierarchyResources instance;

        internal static QHierarchyResources Instance()
        {
            if (instance == null)
            {
                instance = new QHierarchyResources();
            }

            return instance;
        }

        private readonly Dictionary<EM_QHierarchyTexture, Texture2D> textures;

        private readonly Dictionary<EM_QHierarchyTexture, string> resourcesCommon = new Dictionary<EM_QHierarchyTexture, string>()
        {
            {
                EM_QHierarchyTexture.QColorButton,
                "iVBORw0KGgoAAAANSUhEUgAAAAgAAAAQCAYAAAArij59AAAAWUlEQVQoFWP8//8/Az7AhE8SJDcYFLBAHakLpIOB2AGIDwDxWiC+DMQMMAUgSR+QABDAaLACmC8cwFIIAs6HKTiAkAOz4HyYFSA7QcABiA8AMYzPwDgUghoAHO8PN+sTbZ4AAAAASUVORK5CYII="
            },
            {
                EM_QHierarchyTexture.QColorPalette,
                "iVBORw0KGgoAAAANSUhEUgAAAJYAAAA8CAYAAACEhkNqAAADBklEQVR4Ae2dT2sTQRyGZ5uALZaqhxJvevTixasHJR9Mqgc/gDfvkoP4IeqfowcVFPSSBDwYsCBpTY20jTuxC+a3dvKblzk+e1lm5n2n5eFhdguhqRb1FcTrd7cSm3Wtr1d/PdySy092b8ndp7NncnfrzTW5G/b16uN3evdueCGXN+QmRQgkCCBWAg5LOgHE0tnRTBBoiXX6aC+cvdxfqcRxnOeCgJdAS6zq3v2wqEVq5Ir3OI7zXBDwEuja4EYt0Fk9uZRrNAqL8WgpVZzngoCXQOvEisUoUXXj5l+p6jtSeXGSawj8V6zl4y+eVOdyNY/FpsQdAusItB6F/75TLR+L5+9Y8fHIybUOJ+sNgdaJ1byoNxLFe/NC35S4Q2AdgdaJ1Xmw1+osJePlvcWFiYsJtE6si6OsQMBPALH8rEhmEECsDFhE/QS6w+HQnzbJ+ellM5MxnPzMCK9GO5/i36ja9bE31Yp16/jwi9w9GW/L3XCgVz8f6t2rYSyXObFkdBRTBBArRYc1mQBiyegopgggVooOazIBxJLRUUwRQKwUHdZkAoglo6OYIoBYKTqsyQQQS0ZHMUUAsVJ0WJMJIJaMjmKKAGKl6LAmE0AsGR3FFAHEStFhTSbQHQwGcnm7uiR3w7eZ3N15dSJ33+/+kLuzo9dytzNsfQrcv9dXf9Qm3x7ZGf94Gj74wybJiWWAMCxDALHKcGQXQwCxDBCGZQggVhmO7GIIIJYBwrAMAcQqw5FdDAHEMkAYliGAWGU4soshgFgGCMMyBBCrDEd2MQQQywBhWIYAYpXhyC6GAGIZIAzLEECsMhzZxRCoer2e/CVN/cncbJcx3NQ/vrJzR/9yqOdXNjN+ydXodN5fncgYVQf6f8gJk4wfZKK3v5uJjOH1oH9ehxMrAzRRPwHE8rMimUEAsTJgEfUTQCw/K5IZBBArAxZRPwHE8rMimUEAsTJgEfUTQCw/K5IZBBArAxZRPwHE8rMimUEAsTJgEfUTQCw/K5IZBBArAxZRPwHE8rMimUHgD7Cif5j2Lp5yAAAAAElFTkSuQmCC"
            },
            {
                EM_QHierarchyTexture.QComponentUnknownIcon,
                "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAABGElEQVQ4EWNgoBAwQvVLAWk1Isy6BVTzDFkdC5Sj9ujZy/3IEsjsD5+/MixdtJChs7XRESoON4QJWSEuNgc7K0NwRBRDbVMbyBKQS0EuBgPiDGBjYxDg4WLwDQxmqG/tghmC24CP334yZM3fziCfN4nBp3sFw+XHrxg42CGGOLt7Qu3G44Jlx64wHLn5iGFpdiCDnAg/Q9a87WDVIEPEhPhRDIAFIoqgjbosg66sGAOIBtkOMgwGuDjYYUwwjdUAkGYQePT2I8P0PWcYMl1MwHxsBN5ArFyxD+ySTBdjbHrBYngNAKnAZztIHq8BNupyIDV4AV4DOjYdBQciPhOwBiJMw8NJeTAmThqvC3DqQpIgNTfCtMJzJQDmf0F9Rh99OwAAAABJRU5ErkJggg=="
            },
            {
                EM_QHierarchyTexture.QErrorIcon,
                "iVBORw0KGgoAAAANSUhEUgAAAAcAAAAQCAYAAADagWXwAAAANklEQVQYGWP8/fs3Ay7AhEsCJE605H+gYhCGA6J1wnXAGAOhkwVmOZBmRGKDmUQ7iLRAIN9OAA9DBxP0TyMiAAAAAElFTkSuQmCC"
            },
            {
                EM_QHierarchyTexture.QLockButton,
                "iVBORw0KGgoAAAANSUhEUgAAAA0AAAAQBAMAAAA/jegKAAAAAXNSR0IB2cksfwAAAAlwSFlzAAALEwAACxMBAJqcGAAAACdQTFRFAAAA////////////////////////////////////AAAA////////Uw8KpAAAAA10Uk5TAGh7TvX/yICO/UCK/PopRqIAAABFSURBVHicY2CAA0ZFMMXsGuoMolkSGFJAtCgDA6sAkC4F0gUImmNqaGjoDAYGTiAVGgkUCw1dGhqKm4ap4z4KpE/DLQUA21oT++3FRPAAAAAASUVORK5CYII="
            },
            {
                EM_QHierarchyTexture.QMonoBehaviourIcon,
                "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAMUlEQVQ4EWP8//8/AyWABagZZEIhuYYwkasRpm/UAAaG0TAYDQNQfgClgz+wjEEODQAZqgWLOZX9TgAAAABJRU5ErkJggg=="
            },
            {
                EM_QHierarchyTexture.QPrefabIcon,
                "iVBORw0KGgoAAAANSUhEUgAAAAkAAAAQCAYAAADESFVDAAAAd0lEQVQoFY2RUQqAMAxDV/GeHkU8ijfzHv3QphIJm7AWtpbtkWTM3L3NapkBuC9Ba4D3j5rpGSDU8bbcd5lzLNmVINpBdhMb5sxsvdIZ4BVLMzYqMayqfcKAUjI6LKA0VG83ADgoQSYfzBepWkZhcFwwm0I5l+weLU0O7oJcg0oAAAAASUVORK5CYII="
            },
            {
                EM_QHierarchyTexture.QRendererButton,
                "iVBORw0KGgoAAAANSUhEUgAAAAwAAAAQAgMAAABfD3aUAAAAAXNSR0IB2cksfwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAAlQTFRFAAAA+/v7AAAAkrXw8AAAAAN0Uk5TAP9AH454PAAAABpJREFUeJxjYEACoqEhDBKJLQxSmUvwspEAACf1CWWA2r8AAAAAAElFTkSuQmCC"
            },
            {
                EM_QHierarchyTexture.QRestoreButton,
                "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAA3klEQVQ4Ec1RMQ7CMAxMUtj4CSPiATyADzB2QWx8gAlF4geIpTO/YEZIbAz8gxWFuyip3KRR2cDS1fad7dSJttaqkjnnvKS1LpUoU1S+FP53wBobXIFXAGNymfWtwMIdcDfGLAjGgcuGjCC0xlvHjR9AnIFNKyh1C3ENfxJ89gpbihj0DN7X8hmBBvyUh0jIFd6onkEc0wPMB02uUOGU2LRCJ3M/gCfCauQPT4iP/APSVdCiZzoHjsASaICOyT+Igm+Oe4K8hJP3iDsXyIa+AeSlTWSSxukKqT6Y/37AB6sOP8hny1/VAAAAAElFTkSuQmCC"
            },
            {
                EM_QHierarchyTexture.QTreeMapCurrent,
                "iVBORw0KGgoAAAANSUhEUgAAAA4AAAAQCAYAAAAmlE46AAAAQ0lEQVQoFWNgwA+E/wMBUIkwujImdAFi+aMa8YQUCx45dCmUKCFKIzAq36CbQpRGRkZGEbI0QjW9RdY8mgCQQwONDQApiglJmB+fmgAAAABJRU5ErkJggg=="
            },
            {
                EM_QHierarchyTexture.QTreeMapLast,
                "iVBORw0KGgoAAAANSUhEUgAAAA4AAAAQCAYAAAAmlE46AAAAPUlEQVQoFWNgwA+E/wMBUIkwujImdAFi+aMa8YQUCx45dCmUKCFKIzAq36CbwogugMbnBvI50MRGuTQLAQD/rQhHffk54gAAAABJRU5ErkJggg=="
            },
            {
                EM_QHierarchyTexture.QTreeMapLevel,
                "iVBORw0KGgoAAAANSUhEUgAAAA4AAAAQCAYAAAAmlE46AAAAJElEQVQoFWNgwA8k/wMBUIkkujImdAFi+aMa8YTUaOCM8MABAI00BE1+cZ4yAAAAAElFTkSuQmCC"
            },
            {
                EM_QHierarchyTexture.QTreeMapLevel4,
                "iVBORw0KGgoAAAANSUhEUgAAADgAAAAQCAYAAABDebxFAAAATklEQVRIDe2SMQoAMAgDpV/w/29t3QWzpnKOGiHmjJgrb1VJcpa1qc3eadaWNTjwd6AQhKB5AryoOSBpD4IyInMBBM0BSXsQlBGZC9YTfL7XEKcUdfHdAAAAAElFTkSuQmCC"
            },
            {
                EM_QHierarchyTexture.QTreeMapObject,
                "iVBORw0KGgoAAAANSUhEUgAAAA4AAAAQCAYAAAAmlE46AAABnUlEQVQoFdVSTW/aQBR83vX6K9iOkAqmQVUQyoUrUg7cg/qLkTjliDggVfTKZ9rKiT9Q2hjLLsTprMWBWL311Lcar/12Z+fN8xL9N6FUKlXxbQHOCTZmBqTAM/Dr9H6QG8+jPhgM7hqNxnW73b7wPK8mhGBxGKfbb9uX1WoVxnH8FfO8SrwaDoe3rusatu2Srovy0I+e53Zvum6v1/uw2Ww+gfhUJV4WRWEsl0uyrBp1Oh0yTUE/fJ+iICKs6a1Wq8BpTpXIuMLfsEEJgoCiKICqTsfjkegN5i8syrIMH8Sk8feBjFAFFA1KkoTDkyaYIKEByHPOy4ZWFQtd1RSmgA0FNIZIFoZgGCpXybAMKXaoKv5M0v1vprKSxFBQgcEEQwUWCTRrNpvtQNxxedpZvMa72K3X67ZpmTq8gcuY4zivh+Mhm0wm36fT6TjP822VmIdh+LBerxN4cZvNpq1pGvd9/3E0Gn1ZLBbjNE2XEMqrN0eKy5wJf91+v/8ZHbbw3+6jKJojL29O6fpvRKyVoeFZA2Qf9kAGoGX/GH8AjXiXWwSceRAAAAAASUVORK5CYII="
            },
            {
                EM_QHierarchyTexture.QTrimIcon,
                "iVBORw0KGgoAAAANSUhEUgAAAAcAAAAQCAYAAADagWXwAAAAOUlEQVQYGWP8//8/Ay7AhEsCJD5YJOHOR3cQXALdtSgS6JKMIAFkgG4sigJ0SZBGuAJsknCTaSQJAGHZBh0Iaq7CAAAAAElFTkSuQmCC"
            },
            {
                EM_QHierarchyTexture.QVisibilityButton,
                "iVBORw0KGgoAAAANSUhEUgAAABIAAAAQCAYAAAAbBi9cAAAAkElEQVQ4je1SSQqAMAyc+hB/7U30NdJviB5Fn2CkaKVO0yqePDgQQjPJZKH48QwiwmYlxqjkXfUDoqXyXhGdNSHjhZyj8U3gJcUbs6cVR7BJiLjilWIIuGi1ENMxNsPFKoqVXqOIVPNYs6yINEp3ZN5n3uUWN8dO3UQ9tidqKhwAWBJZlCbnamzdqw/5HQDYACKZHqSWDM0rAAAAAElFTkSuQmCC"
            },
            {
                EM_QHierarchyTexture.QVisibilityOffButton,
                "iVBORw0KGgoAAAANSUhEUgAAABIAAAAQCAYAAAAbBi9cAAABdUlEQVQ4Ea3TvS8EQRjH8dvLHZGoLvQqQaKReKlEp6RA7Q9QafwDGp1EIgqRy3mJhASJSiMaCi4Roj2rOAonkmtEgvX9ze6zVrKh2Sf5zPPM7Mxkd+bOC4Igl0Xks9hEe2S+0TibVlLersCY/Bv2Rs/MnMR2tGKAfIkafJxjET1IDx12pI+sKCOPLdzhAu9QKC+hCFvncrKzzEOLfQp71kJdwpk9JB+hFTYnZ8VCYtIrdR2H0cQ58iza8QSLFQoPbg+dkb57LPHhV9RDmMAe1tCNN5zAop9C81zoRoqRcCRs66R7TEG/2BkovDC5Vi+htS7UucV12HXtIG0J82hiGmUodLsWjxRV69gZ6UCP7ePJp2iDzqAXTVSg+Zu4gS7A1seHrQHdwiq+oNCh72AdDSh2oZ+GDj7eRPWvTvRwmKzFNXxC8YAN+NDVx7dF7fZI28jGCkzqQCf06RrvwgsOor7N/fN/9MFBNuLDDAufNILRsPvTfgP0LsP1SIPKHwAAAABJRU5ErkJggg=="
            },
            {
                EM_QHierarchyTexture.QTreeMapLine,
                "iVBORw0KGgoAAAANSUhEUgAAAA4AAAAQCAMAAAARSr4IAAAACVBMVEX///8AAAD///9+749PAAAAAnRSTlMAE/ItjOYAAAAWSURBVHgBY6AbYEQAEJcJDhjRZWkJABQbACw6WoebAAAAAElFTkSuQmCC"
            }
        };

        private readonly Dictionary<EM_QHierarchyTexture, string> resourcesDark = new Dictionary<EM_QHierarchyTexture, string>()
        {
            {
                EM_QHierarchyTexture.QCheckBoxChecked,
                "iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAABV0lEQVQoFWO0sLD4zwAETEyMDECEF/wDqvwHIoCABUS42wkwlKRKM3CwM4G4OMGPn/8YemY/Zdh56APQIqA1JSnSDDxczAwszIx4MUgNSC3YdSDncXDgtundhz8Mdx7+hLsCpBakB7cOqNJpS18zpFY9ZFi26R1cM4iBofHM5W/AAICouXzzO8PeY5/BHE1lDogglETROHf1G4bSjicMC9e/Zfjz5z9D9+yXYGXOVrwMhtpcKBrBoQoT0dfgYljG9I5h0bq3DM9f/WZ4/PwXAyfQTxlRojAlcBrFRhNdLoaMSIiiPUc/gRUlhwoziAiimA8WR9EIEgn1EmTQVOFg+A+MZxV5doZAN0GwQnQC0yigimmNcgy7jnxikBZnA8YZuhYInwWUgn78+MfAw82MosLNhg+FD+OA1IL0MIHSXs+cpwxfvv1l+PP3P14MUgNSC9LDSG4iBwCgfYRJ3KYMwgAAAABJRU5ErkJggg=="
            },
            {
                EM_QHierarchyTexture.QCheckBoxUnchecked,
                "iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAAgElEQVQoFe2SywkAIQxE46cHO/Bu/53YgdaguPsCgpd1hb1uIBjxDSNhTEppyF3GGI7XGkNx8ZAhBIkxinNuK+y9S85ZSilicULkvVdX7k8NA8u7xeLNaf3GZFW4PpzOv3CzqW/LIRGnNVlL9ohRa02Ydw0DC6NZJXu11iNTRNQFcsdGKGm8LNQAAAAASUVORK5CYII="
            },
            {
                EM_QHierarchyTexture.QDragButton,
                "iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAfElEQVQ4EWM0NjZmoCZgoqZhILNGoIEsaGFoDuRroIkR4t4AKjgJU4RuoMb///8XwCSJoRkZGROA6nAaeMPBwYEYc+BqDh48CHIhHGC48MCBA3BJYhhAF4KCaNSFiMDSoHoYQpMBwgrCLLyxDIoteIwRNgtTxQgsHAa/lwH5tiOYn8m38AAAAABJRU5ErkJggg=="
            }
        };

        private readonly Dictionary<EM_QHierarchyTexture, string> resourcesLight = new Dictionary<EM_QHierarchyTexture, string>()
        {
            {
                EM_QHierarchyTexture.QCheckBoxChecked,
                "iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAABOUlEQVQoFY2Sva6CQBCFz4XVQGFjgiZQGKiJnYmFBQVPAo9l7RvQUZAgvAChpqAiGkNDpQLX2bjkkhuMW+zM7pwv85P58X2/x+swxiBJErmTp+s6PJ9PHmd0Hw4HrFYrzGazSYgCj8cDl8sFSZJQIgZN06AoykeIgrIsc+27Ognz+XwSapoGVVUNcdJSS5+besnDMMTxeESapgNMzj+wKArQEOiUZYk8z7mv6zq34hqBURThdDrhfD6jbVsEQcB1tm3DNE3BcMunKn42mw0vKY5j1HWN2+3G+3ddV0gGO8poWRaESJToOA4Wi8UACGcE0ud+v4dhGOj7Huv1GrvdTmhHdlSqiHiehyzLsFwuJ7eJ0QTv9ztUVRUct9vtdvQWD9ISw2j3rtfr1ytHWmJ4qe/dmyxLZPu75L/vGnGpeAWI1gAAAABJRU5ErkJggg=="
            },
            {
                EM_QHierarchyTexture.QCheckBoxUnchecked,
                "iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAAjklEQVQoFe2STRJDERCEOwzlApzGbVzVabwboCRp9ezeT5J1ZmGU+Roz1Y+U0hPvEBEopbg9jTEGeu+zLlxjjAghwBhzKmKhtYZSCnLOfEjgvYdz7lLEotZ6svvvFKy1t6IFkGVL100t+iD/hQdDWUe/D4c2qrWui24zWWqE3tu27WPLkaVmenX33lcmfwFMazT7V5IT7wAAAABJRU5ErkJggg=="
            },
            {
                EM_QHierarchyTexture.QDragButton,
                "iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAATklEQVQ4EWM8ceIEAzUBEzUNA5k1aiDlIcqCxYj/WMTwCTEiS7Js27oVmU8xmxFLOhx1IWnBSv0wxGI/SjLAIo9XaDQv4w0eoiQHfxgCAGQPE/BDNfMZAAAAAElFTkSuQmCC"
            }
        };

        private readonly Dictionary<EM_QHierarchyColor, Color> colors;

        private readonly Dictionary<EM_QHierarchyColor, Color> colorsDarkSkin = new Dictionary<EM_QHierarchyColor, Color>()
        {
            {
                EM_QHierarchyColor.BackgroundDark, new Color(0.15f, 0.15f, 0.15f)
            },
            {
                EM_QHierarchyColor.Background, new Color(0.22f, 0.22f, 0.22f)
            },
            {
                EM_QHierarchyColor.Gray, new Color(0.8f, 0.8f, 0.8f)
            },
            {
                EM_QHierarchyColor.GrayLight, new Color(0.8f, 0.8f, 0.8f)
            },
            {
                EM_QHierarchyColor.GrayDark, new Color(0.4f, 0.4f, 0.4f)
            }
        };

        private readonly Dictionary<EM_QHierarchyColor, Color> colorsLightSkin = new Dictionary<EM_QHierarchyColor, Color>()
        {
            {
                EM_QHierarchyColor.BackgroundDark, new Color(0.88f, 0.88f, 0.88f)
            },
            {
                EM_QHierarchyColor.Background, new Color(0.761f, 0.761f, 0.761f)
            },
            {
                EM_QHierarchyColor.Gray, new Color(0.2f, 0.2f, 0.2f)
            },
            {
                EM_QHierarchyColor.GrayLight, new Color(0.1f, 0.1f, 0.1f)
            },
            {
                EM_QHierarchyColor.GrayDark, new Color(0.55f, 0.55f, 0.55f)
            }
        };

        private QHierarchyResources()
        {
            textures = new Dictionary<EM_QHierarchyTexture, Texture2D>();

            foreach (var resourcePair in resourcesCommon)
            {
                var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false, false)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                texture.LoadImage(Convert.FromBase64String(resourcePair.Value));
                textures.Add(resourcePair.Key, texture);
            }

            var resources = EditorGUIUtility.isProSkin ? resourcesDark : resourcesLight;

            foreach (var resourcePair in resources)
            {
                var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false, false)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                texture.LoadImage(Convert.FromBase64String(resourcePair.Value));
                textures.Add(resourcePair.Key, texture);
            }

            colors = EditorGUIUtility.isProSkin ? colorsDarkSkin : colorsLightSkin;
        }

        internal Texture2D GetTexture(EM_QHierarchyTexture hierarchyTextureName)
        {
            return textures[hierarchyTextureName];
        }

        internal Color GetColor(EM_QHierarchyColor hierarchyColor)
        {
            return colors[hierarchyColor];
        }
    }
}
