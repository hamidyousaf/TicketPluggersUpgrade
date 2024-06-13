using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TicketServer.Helpers;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Helpers.Pagination;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Services
{
    public sealed class VenueService : IVenueService
    {
        private readonly IVenueRepository _venueRepository;
        private readonly IFileHelper _fileHelper;
        private IMapper _mapper;
        private readonly IHostingEnvironment _environment;

        public VenueService(
            IVenueRepository venueRepository,
            IMapper mapper,
            IFileHelper fileHelper,
            IHostingEnvironment environment)
        {
            _venueRepository = venueRepository;
            _mapper = mapper;
            _fileHelper = fileHelper;
            _environment = environment;
        }
        public async Task<bool> IsVenueExist(int venueId)
        {
            return await _venueRepository.GetAll().Select(x => x.Id).AnyAsync(x => x == venueId);
        }
        public async Task<List<VenueLite>> SearchSuggestEvents(string searchText)
        {
            return await _venueRepository.SearchSuggestVenues(searchText).ToListAsync();
        }
        public async Task<List<SeatingSection>> GetSections(int venueId)
        {
            var results = await _venueRepository.GetVenueByIdFromMongoDB(venueId);
            List<SeatingSection> seatingSections = new List<SeatingSection>();
            if (results is not null)
            {
                seatingSections = results.seatingZones.SelectMany(x => x.seatingSections).ToList();
            }
            return seatingSections;
        }

        public async Task<ResponseModel> GetVenueInfo(int venueId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get venue by id.
            var venue = await _venueRepository.Get(venueId);
            if (venue is null || venue.IsDeleted)
            {
                return new ResponseModel()
                {
                    Message = "There is no such a venue."
                };
            }
            var response = new GetVenueByInfoDto()
            {
                Id = venue.Id,
                ImageURL = venue.VenueImage,
                Name = venue.VenueName + " " + venue.City,
                Location = venue.City + " , " + venue.Street + " , " + venue.CountryCode,
                VenueImage = venue.VenueImage
            };

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = response
            };
        }

        public async Task<ResponseModel> GetAllVenue(GetAllVenueRequest request, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var data = _venueRepository.GetAll();
            // Add pagination
            PaginationModel<Venue> model = new PaginationModel<Venue>();
            if ((request.PageSize) > 0)
            {
                var result = new PagedList<Venue>(data, request.PageIndex, request.PageSize, false);
                model.PagedContent = result;
                model.PagingFilteringContext.LoadPagedList(result);
            }
            return new ResponseModel()
            {
                IsSuccess = true,
                Data = model
            };
        }

        public async Task<ResponseModel> GetAllVenueBySearch(string searchString, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var data = await _venueRepository
                .GetAll()
                .Where(x => x.VenueName.ToLower().Contains(searchString.ToLower()))
                .Select(x => new GetVenueByInfoDto()
                {
                    Id = x.Id,
                    ImageURL = x.VenueImage,
                    Name = x.VenueName + " " + x.City,
                    Location = x.City + " , " + x.Street + " , " + x.CountryCode,
                    VenueImage = x.VenueImage
                })
                .Take(10)
                .ToListAsync(ct);

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }

        public async Task<ResponseModel> GetVenueConfiguration(string id, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var data = await _venueRepository.GetVenueConfiguration(Convert.ToInt32(id), ct);
            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }
        public async Task<ResponseModel> GetVenueConfigurationForAdmin(string id, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };
            // get venue by id.
            var venue = await _venueRepository.Get(Convert.ToInt32(id));

            var data = await _venueRepository.GetVenueConfiguration(Convert.ToInt32(id), ct);
            if (data is not null && venue is not null)
            {
                data.Location = venue.City;
                data.Country = venue.CountryCode;
                data.Street = venue.Street;
                data.Venue = venue.VenueName;
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }

        public async Task<ResponseModel> GetVenueMap(int venueId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var data = await _venueRepository.GetVenueMap(venueId, ct);

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }

        public async Task<ResponseModel> GetVenuesWithMapInfo(string searchText, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get vanues from mongo db.
            var venueHaveMap = await _venueRepository.GetVanuesFromMongoDB(ct);
            var venues = _venueRepository
                .GetReadOnlyList()
                .Where(x => x.VenueName.ToLower().Contains(searchText.ToLower()) && venueHaveMap.Select(a => a.venueId).Contains(x.Id))
                .ProjectTo<VenueLite>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = venues
            };
        }

        public async Task<ResponseModel> CreateVenue(VenueDto venue, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };


            if (venue.Id == 0)
            {
                var venueInfo = new Venue()
                {
                    Venue_ID = Guid.NewGuid().ToString(),
                    VenueName = venue.VenueName,
                    Street = venue.Street,
                    City = venue.City,
                    StateCode = venue.StateCode,
                    CountryCode = venue.CountryCode,
                    Latitude = venue.Latitude,
                    Longitude = venue.Longitude,
                    ZipCode = venue.ZipCode,
                    TimeZone = venue.TimeZone,
                    VenueURL = "",
                    VenueImage = venue.VenueImage != null ? await _fileHelper.UploadFile(@"static//venues", venue.VenueImage) : null,
                    Active = venue.Active,
                    IsCustom = true,
                    IsDeleted = false
                };
                await _venueRepository.Add(venueInfo);
                return new ResponseModel()
                {
                    IsSuccess = true,
                    Message = "Venue added."
                };
            }
            else
            {
                var venueInfo = await _venueRepository.Get(venue.Id);
                if (venueInfo is null) return new ResponseModel()
                {
                    IsSuccess = true,
                    Message = "No venue exists."
                };
                venueInfo.VenueName = venue.VenueName;
                venueInfo.Street = venue.Street;
                venueInfo.City = venue.City;
                venueInfo.StateCode = venue.StateCode;
                venueInfo.CountryCode = venue.CountryCode;
                venueInfo.Latitude = venue.Latitude;
                venueInfo.Longitude = venue.Longitude;
                venueInfo.ZipCode = venue.ZipCode;
                venueInfo.TimeZone = venue.TimeZone;
                venueInfo.VenueImage = venue.VenueImage != null ? await _fileHelper.UploadFile(@"static//venues", venue.VenueImage) : venueInfo.VenueImage;
                venueInfo.Active = venue.Active;
                _venueRepository.Change(venueInfo);
            }
            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Venue updated."
            };
        }

        public async Task<ResponseModel> GetCustomVenues(CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var data = await _venueRepository
                .GetReadOnlyList()
                .Where(x => x.IsCustom == true && !x.IsDeleted)
                .ProjectTo<VenueLite>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }

        public async Task<ResponseModel> DeleteCustomVenue(long id, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get venue by id.
            var venue = await _venueRepository.Get(Convert.ToInt32(id));
            if (venue is null)
            {
                return new ResponseModel()
                {
                    Message = "There is no such a venue."
                };
            }

            venue.IsDeleted = true;
            await _venueRepository.Change(venue);
            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Venue deleted."
            };
        }

        public async Task<ResponseModel> UpdateCustomeVenueStatus(long id, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };


            // get venue by id.
            var venue = await _venueRepository.Get(Convert.ToInt32(id));
            if (venue is null || venue.IsDeleted)
            {
                return new ResponseModel()
                {
                    Message = "There is no such a venue."
                };
            }
            venue.Active = !venue.Active;
            await _venueRepository.Change(venue);
            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Venue status changed."
            };
        }

        public async Task<ResponseModel> DeleteVenue(GetVenuesWithMapInfoRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var result = _venueRepository.DeleteVenue(request.Id);
            if (!result)
            {
                return new ResponseModel()
                {
                    Message = "Something went wrong."
                };
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Venue deleted."
            };
        }

        public async Task<ResponseModel> AddZone(ZoneRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var result = await _venueRepository.AddZone(request, ct);
            if (!result)
            {
                return new ResponseModel()
                {
                    Message = "Something went wrong."
                };
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Zone added."
            };
        }

        public async Task<ResponseModel> DeleteZone(DeleteZoneRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var result = await _venueRepository.DeleteZone(request, ct);
            if (!result)
            {
                return new ResponseModel()
                {
                    Message = "Something went wrong."
                };
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Zone deleted."
            };
        }

        public async Task<ResponseModel> ActivateVenue(ActivateVenueRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };


            // get venue by id.
            var venue = await _venueRepository.Get(Convert.ToInt32(request.Id));
            if (venue is null)
            {
                return new ResponseModel()
                {
                    Message = "There is no such a venue."
                };
            }
            venue.Active = true;
            await _venueRepository.Change(venue);
            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Venue status changed."
            };
        }

        public async Task<ResponseModel> AddSection(AddSectionRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var result = await _venueRepository.AddSection(request, ct);
            if (!result)
            {
                return new ResponseModel()
                {
                    Message = "Something went wrong."
                };
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Section deleted."
            };
        }

        public async Task<ResponseModel> AddStadium(AddStadiumRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // check file available.
            if (request.ImageFile is null || request.File is null) return new ResponseModel()
            {
                Message = "File should not be empty."
            };

            string folderName = "Static";
            string SubfolderName = "SeatMaps";
            string webRootPath = _environment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            newPath = Path.Combine(newPath, SubfolderName);
            var docId = Guid.NewGuid().ToString();
            var filePath = Path.Combine(newPath, docId.ToString() + Path.GetExtension(request.ImageFile.FileName.ToLower()));
            using (var stream = System.IO.File.Create(filePath))
            {
                await request.ImageFile.CopyToAsync(stream);
                request.ImageURL = docId + Path.GetExtension(request.ImageFile.FileName.ToLower());
            }
            StreamReader reader = new StreamReader(request.File.OpenReadStream());
            string record = "";
            string record2 = "";
            var sectionId = request.VenueId;
            int i = 1;
            List<string> dsf = new List<string>();
            List<Metas> measures = new List<Metas>();
            List<SeatingSection> seatingSections = new List<SeatingSection>();
            List<SeatingZone> seatingZones = new List<SeatingZone>();
            VenueConfigurationCreate venue = new VenueConfigurationCreate();
            Dictionary<string, object> myDictionary = new Dictionary<string, object>();
            string SvgText = "";
            string SvgTextApp = "";
            while (!reader.EndOfStream)
            {
                record = reader.ReadLine();
                record2 = record;
                while (record != null)
                {
                    Metas metas = new Metas();
                    SeatingSection seatingSection = new SeatingSection();
                    dynamic section = new JObject();
                    if (record.Contains("<svg "))
                    {
                        record = record.Replace("width", "widths").Replace("height", "heights");
                        record2 = record;
                    }
                    if (record.Contains("<path ") && record.Contains("/>"))
                    {
                        if (record.Contains("fill=\"#e8e8e8\""))
                        {
                            record = record.Replace("id", "ids");
                            record2 = record;
                            int pFrom = record.IndexOf(" d=") + 4;
                            int pTo = record.IndexOf("\"", pFrom);
                            if (pFrom > 0 && pTo > pFrom)
                            {
                                metas.p = record.Substring(pFrom, pTo - pFrom);
                            }
                            else
                            {
                                return new ResponseModel()
                                {
                                    Message = $"Please Redraw,No d attribute in {record}."
                                };
                            }

                            metas.p = record.Substring(pFrom, pTo - pFrom);
                            record = record.Remove(pFrom, pTo - pFrom);
                            metas.section = (sectionId + i).ToString();

                            var ss = metas.p + "\" class=\"map__path__selectable_hover\" data-bs-toggle=\"modal\" data-bs-target=\"#add-section\" onClick=handleMapSectionSelect(" + metas.section + ") id=\"" + metas.section + "\" fill=\"#a9a7a7\" style=\"pointer-events: auto;";
                            record = record.Insert(pFrom, ss);
                            //----for app--
                            record2 = record2.Remove(pFrom, pTo - pFrom);
                            ss = metas.p + "\" class=\"map__path__selectable_hover\" data-bs-toggle=\"modal\" data-bs-target=\"#add-section\" onPress=handleMapSectionSelect(" + metas.section + ") id=\"" + metas.section + "\" fill=\"#a9a7a7\" style=\"pointer-events: auto;";
                            record2 = record2.Insert(pFrom, ss);
                            //--end  for app
                            pFrom = record.IndexOf("transform") + 11;
                            pTo = record.IndexOf(")\"") + 1;
                            if (pFrom > 0 && pTo > pFrom)
                                metas.transform = record.Substring(pFrom, pTo - pFrom);
                            metas.fill = "#dbdbdb";
                            metas.t = "true";

                            myDictionary.Add(metas.section, metas);
                            seatingSection.id = Convert.ToInt32(sectionId + i);
                        }
                        else
                        {
                            int pFrom = record.IndexOf(" d=") + 4;
                            int pTo = record.IndexOf("\"", pFrom);
                            if (pFrom > 0 && pTo > pFrom)
                            {
                                metas.p = record.Substring(pFrom, pTo - pFrom);
                            }
                            else
                            {
                                return new ResponseModel()
                                {
                                    Message = $"Please Redraw,No d attribute in {record}."
                                };
                            }

                            pFrom = record.IndexOf("transform") + 11;
                            pTo = record.IndexOf(")\"") + 1;
                            if (pFrom > 0 && pTo > pFrom)
                                metas.transform = record.Substring(pFrom, pTo - pFrom);
                            metas.fill = "#fff";
                            metas.t = "false";
                            metas.section = Convert.ToInt32(sectionId + i).ToString();
                            myDictionary.Add(metas.section, metas);
                        }
                    }
                    else
                    {
                        if (record.Contains("transform"))
                        {
                            return new ResponseModel()
                            {
                                Message = $"Please Redraw,Other tools with transform is not supported {record}."
                            };
                        }
                    }
                    SvgText = SvgText + record;
                    i = i + 1;
                    record = reader.ReadLine();
                    record2 = record;
                }
            }
            int zonId = 1;
            foreach (var zone in request.ZoneNames.Split(','))
            {
                SeatingZone seatingZone = new SeatingZone();
                seatingZone.id = Convert.ToInt32(request.VenueId + zonId);
                seatingZone.name = zone;
                seatingZone.seatingSections = seatingSections;
                seatingZones.Add(seatingZone);
                zonId = zonId + 1;
            }
            var output = Newtonsoft.Json.JsonConvert.SerializeObject(myDictionary);
            venue.sectionZoneMetas = output;
            venue.venueId = Convert.ToInt32(request.VenueId);
            venue.staticImageUrl = request.ImageURL;
            venue.description = request.description;
            venue.seatingZones = seatingZones;
            venue.active = true;// not deleted
            venue.svg = SvgText;
            venue.svgApp = SvgTextApp;
            var reponse = _venueRepository.CreateVenueInMongoDB(venue);
            if (reponse != null)
            {
                return new ResponseModel()
                {
                    IsSuccess = true,
                    Message = $"Successfully added."
                };
            }
            else
            {
                return new ResponseModel()
                {
                    Message = $"Already exist."
                };
            }
        }
    }
}